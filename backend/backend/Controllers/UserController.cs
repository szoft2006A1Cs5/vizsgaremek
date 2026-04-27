using System.ComponentModel.DataAnnotations;
using backend.Contexts;
using backend.DTOs.User;
using backend.Models;
using backend.Services;
using backend.Services.ResourceService;
using backend.VisibilityFiltering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Context _context;
        private readonly AuthService _authSrv;
        private readonly TimeProvider _timePrv;
        private readonly IResourceService _resSrv;

        public UserController(Context ctx, AuthService authSrv, IResourceService resSrv, TimeProvider timePrv)
        {
            _context = ctx;
            _authSrv = authSrv;
            _resSrv = resSrv;
            _timePrv = timePrv;
        }

        /// <summary>
        /// Visszaadja az adott azonositoju felhasznalo adatait
        /// </summary>
        /// <param name="id">A felhasznalo azonositoja</param>
        /// <returns>
        /// 404-et, ha nincs ilyen azonositoju felhasznalo.
        /// 200-at + egy bejelentkezett felhasznalo alapjan szurt felhasznalo adatait.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var authUser = await _authSrv.GetUser(User);

            var user = await _context.Users
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(x => x.Rentals)
                .ThenInclude(x => x.Vehicle)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.Rentals)
                .Include(x => x.Notifications
                    .OrderByDescending(y => y.TimeSent)
                )
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (user == null) return NotFound();

            return Ok(user.FilterSerialize(authUser));
        }

        /// <summary>
        /// Visszaadja a bejelentkezett felhasznalo adatait.
        /// </summary>
        /// <returns>
        /// 401-et, ha nincs bejelentkezett felhasznalo.
        /// 200-at maskepp.
        /// (Elmeletileg lehetseges a 404 is,
        /// ha valahogy a bejelentkezett felhasznalo letezne is meg nem is,
        /// de gyakorlatban tulajdonkeppen lehetetlen.)
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAuthUser()
        {
            var authUser = await _authSrv.GetUser(User);

            if (authUser == null) return Unauthorized();

            return await GetUserById(authUser.Id);
        }

        /// <summary>
        /// Frissiti a megadott azonositoju felhasznalo adatait.
        /// </summary>
        /// <param name="id">A felhasznalo azonositoja.</param>
        /// <param name="dto">A modositott felhasznaloi adatok.</param>
        /// <returns>
        /// 400-at, ha a megadott adatok hibasak.
        /// 
        /// 401-et, ha nincs bejelentkezett felhasznalo.
        /// 
        /// 403-at, ha a bejelentkezett felhasznalo
        /// nem a sajat fiokjat szerkeszti (es nem admin), illetve ha
        /// a felhasznalo nem adta meg helyesen a jelszavat.
        /// 
        /// 404-et, ha nincs ilyen azonositoju felhasznalo.
        /// 
        /// 409-et, ha a megadott adatok utkoznek egy masik felhasznalo
        /// adataival.
        ///
        /// 200-at, ha a felhasznalo adatai sikeresen frissultek.
        /// </returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserById(int id, [FromBody] UserModificationDTO dto)
        {
            if (!dto.CheckValid(_timePrv))
                return BadRequest(new { Error = "A megadott adatok hibásak!" });
            
            var authUser = await _authSrv.GetUser(User);

            if (authUser == null) return Unauthorized(); 
            if (authUser.Id != id && authUser.Role != UserRole.Administrator) return Forbid();

            // Duplan nezzuk az authUser id-t, de megeri, mert igy atugorhatunk egy adatbazis lekerdezest,
            // habar a beincludeolt tablak inkozisztensek, habar ez egy update-nel talan nem olyan nagy gond.
            var user = authUser.Id == id ? authUser : await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return NotFound();
            
            if (authUser.Role != UserRole.Administrator &&
                !_authSrv.VerifyPassword(dto.PreviousPassword, user)) return Forbid();

            var phone = dto.Phone.Substring(2);
            if (_context.Users.Any(x => x != user && (x.Email == dto.Email ||
                                                      x.Phone.EndsWith(phone) ||
                                                      x.IdCardNumber == dto.IdCardNumber ||
                                                      (!string.IsNullOrWhiteSpace(dto.DriversLicenseNumber) ? 
                                                          x.DriversLicenseNumber == dto.DriversLicenseNumber : 
                                                          false))))
                return Conflict();
            
            var userProps = typeof(User).GetProperties().Where(x => !(new[] {
                nameof(Models.User.Password),
                nameof(Models.User.Salt),
                nameof(Models.User.Role),
                nameof(Models.User.Balance)
            }.Contains(x.Name))).ToList();
            
            foreach (var dtoProp in typeof(UserModificationDTO).GetProperties())
            {
                var userProp =
                    userProps.FirstOrDefault(x => x.Name == dtoProp.Name &&
                                                  x.PropertyType == dtoProp.PropertyType);
                
                if (userProp != null)
                    userProp.SetValue(user, dtoProp.GetValue(dto));
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var (pass, salt) = _authSrv.GeneratePasswordHashSalt(dto.Password);
                user.Password = pass;
                user.Salt = salt;
            }

            if (authUser.Role == UserRole.Administrator)
            {
                user.Role = dto.Role ?? user.Role;
                user.Balance = dto.Balance ?? user.Balance;
            }

            await _context.SaveChangesAsync();
            
            return Ok(user.FilterSerialize(authUser));
        }
        
        /// <summary>
        /// Feltolti a megadott azonositoju felhasznalo egyenleget
        /// </summary>
        /// <param name="id">A felhasznalo azonositoja</param>
        /// <param name="amount">A feltoltendo osszeg</param>
        /// <returns>
        /// 400-at, ha 0 vagy az alatti osszeget akarunk feltolteni.
        /// 
        /// 401-et, ha nincs bejelentkezett felhasznalo.
        /// 
        /// 403-at, ha a bejelentkezett felhasznalo
        /// nem a sajat fiokjara tolt fel penzt (es nem admin).
        ///
        /// 404-et, ha nincs ilyen azonositoju felhasznalo.
        ///
        /// 200-at, ha az osszeg sikeresen "feltoltodott".
        /// </returns>
        [HttpPut("{id}/Deposit")]
        public async Task<IActionResult> Deposit(int id, [FromBody] int amount = 0)
        {
            var authUser = await _authSrv.GetUser(User);

            if (authUser == null) return Unauthorized();
            if (authUser.Id != id && authUser.Role != UserRole.Administrator) return Forbid();

            if (amount <= 0) return BadRequest();

            var user = authUser.Id == id ? authUser : await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return NotFound();

            user.Balance += amount;
            await _context.SaveChangesAsync();

            return Ok(user.FilterSerialize(authUser));
        }

        [HttpPut("{id}/Image")]
        public async Task<IActionResult> UpdateUserImageById(int id, IFormFile? file)
        {
            var authUser = await _authSrv.GetUser(User);

            if (authUser == null) return Unauthorized();
            if (authUser.Id != id && authUser.Role != UserRole.Administrator) return Forbid();

            var user = authUser.Id == id ? authUser : await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return NotFound();

            string? path = null;
            if (file != null)
            {
                path = await _resSrv.Store(file);
                if (path == null) return StatusCode(500);
            }

            if (user.ProfilePicPath != null)
                _resSrv.Delete(user.ProfilePicPath);

            user.ProfilePicPath = path;

            await _context.SaveChangesAsync();

            return Ok(user.FilterSerialize(authUser));
        }

        /// <summary>
        /// Visszaadja az adott azonositoju felhasznalo ertesiteseit.
        /// </summary>
        /// <param name="userId">A felhasznalo azonositoja.</param>
        /// <returns>
        /// 401-et, ha nincs bejelentkezett felhasznalo.
        /// 
        /// 403-at, ha a bejelentkezett felhasznalo
        /// nem a sajat ertesiteseit keri le (es nem admin).
        ///
        /// 200-at + az ertesiteseket maskepp.
        /// </returns>
        [HttpGet("{userId}/Notification")]
        public async Task<IActionResult> GetNotificationsForUID(int userId)
        {
            var authUser = await _authSrv.GetUser(User);

            if (authUser == null) return Unauthorized();
            if (authUser.Id != userId && authUser.Role != UserRole.Administrator) return Forbid();

            return Ok(
                (await _context.Notifications
                    .Where(x => x.UserId == userId)
                    .ToListAsync())
                .FilterSerialize(authUser)
            );
        }

        /// <summary>
        /// Lekeri egy adott felhasznalo egy adott ertesiteset.
        /// </summary>
        /// <param name="userId">A felhasznalo azonositoja.</param>
        /// <param name="notificationId">Az ertesites azonositoja.</param>
        /// <returns>
        /// 401-et, ha nincs bejelentkezett felhasznalo.
        /// 
        /// 403-at, ha a bejelentkezett felhasznalo
        /// nem a sajat ertesiteset keri le (es nem admin).
        ///
        /// 404-et, ha nincs ilyen azonositoju felhasznalo/ertesites.
        ///
        /// 200-at + az ertesitest maskepp.
        /// </returns>
        [HttpGet("{userId}/Notification/{notificationId}")]
        public async Task<IActionResult> GetNotificationByUIDAndId(int userId, int notificationId)
        {
            var authUser = await _authSrv.GetUser(User);

            if (authUser == null) return Unauthorized();
            if (authUser.Id != userId && authUser.Role != UserRole.Administrator) return Forbid();

            var message = await _context.Notifications.FirstOrDefaultAsync(x => x.UserId == userId && x.NotificationId == notificationId);
            if (message == null) return NotFound();

            return Ok(message.FilterSerialize(authUser));
        }

        /// <summary>
        /// Kitorli egy adott felhasznalo adott ertesiteset.
        /// </summary>
        /// <param name="userId">A felhasznalo azonositoja.</param>
        /// <param name="notificationId">Az ertesites azonositoja.</param>
        /// <returns>
        /// 401-et, ha nincs bejelentkezett felhasznalo.
        /// 
        /// 403-at, ha a bejelentkezett felhasznalo
        /// nem a sajat ertesiteset probalja torolni (es nem admin).
        ///
        /// 404-et, ha nincs ilyen azonositoju felhasznalo/ertesites.
        ///
        /// 204-et, ha az ertesites sikeresen torolve lett.
        /// </returns>
        [HttpDelete("{userId}/Notification/{notificationId}")]
        public async Task<IActionResult> DeleteNotificationByUIDAndId(int userId, int notificationId)
        {
            var authUser = await _authSrv.GetUser(User);

            if (authUser == null) return Unauthorized();
            if (authUser.Id != userId && authUser.Role != UserRole.Administrator) return Forbid();

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.UserId == userId && x.NotificationId == notificationId);
            if (notification == null) return NotFound();

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Kitorli egy adott felhasznalo osszes ertesiteset.
        /// </summary>
        /// <param name="userId">A felhasznalo azonositoja</param>
        /// <returns>
        /// 401-et, ha nincs bejelentkezett felhasznalo.
        /// 
        /// 403-at, ha a bejelentkezett felhasznalo
        /// nem a sajat ertesiteseit probalja torolni (es nem admin).
        ///
        /// 204-et, ha az ertesitesek sikeresen torolve lettek.
        /// </returns>
        [HttpDelete("{userId}/Notification")]
        public async Task<IActionResult> DeleteNotificationsByUID(int userId)
        {
            var authUser = await _authSrv.GetUser(User);

            if (authUser == null) return Unauthorized();
            if (authUser.Id != userId && authUser.Role != UserRole.Administrator) return Forbid();

            _context.Notifications.RemoveRange(_context.Notifications.Where(x => x.UserId == userId));
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
