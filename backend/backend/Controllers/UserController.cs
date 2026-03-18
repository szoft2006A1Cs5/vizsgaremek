using System.ComponentModel.DataAnnotations;
using backend.Contexts;
using backend.DTOs.User;
using backend.Models;
using backend.Services;
using backend.Services.ResourceService;
using backend.VisibilityFiltering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Context _context;
        private readonly AuthService _authSrv;
        private readonly IResourceService _resSrv;

        public UserController(Context ctx, AuthService authSrv, IResourceService resSrv)
        {
            _context = ctx;
            _authSrv = authSrv;
            _resSrv = resSrv;
        }

        // GET api/<UserController>/5
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
                .Include(x => x.Notifications)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (user == null) return NotFound();

            return Ok(user.FilterSerialize(authUser));
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthUser()
        {
            var authUser = await _authSrv.GetUser(User);

            if (authUser == null) return Unauthorized();

            return await GetUserById(authUser.Id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserById(int id, [FromBody] UserModificationDTO dto)
        {
            if (!dto.CheckValid())
                return BadRequest(new { Error = "A megadott adatok hibásak!" });
            
            var authUser = await _authSrv.GetUser(User);

            if (authUser == null) return Unauthorized(); 
            if (authUser.Id != id && authUser.Role != UserRole.Administrator) return Forbid();

            // Duplan nezzuk az authUser id-t, de megeri, mert igy atugorhatunk egy adatbazis lekerdezest.
            var user = authUser.Id == id ? authUser : await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return NotFound();
            
            if (authUser.Role != UserRole.Administrator &&
                !_authSrv.VerifyPassword(dto.PreviousPassword, user)) return Forbid();

            var phone = dto.Phone.Substring(2);
            if (_context.Users.Any(x => x != user && (x.Email == dto.Email ||
                                                      x.Phone.EndsWith(phone) ||
                                                      x.IdCardNumber == dto.IdCardNumber ||
                                                      x.DriversLicenseNumber == dto.DriversLicenseNumber)))
                return Conflict();
            
            var userProps = typeof(User).GetProperties();
            foreach (var dtoProp in typeof(UserModificationDTO).GetProperties())
            {
                var userProp =
                    userProps.FirstOrDefault(x => x.Name == dtoProp.Name &&
                                                  x.PropertyType == dtoProp.PropertyType);
                
                if (userProp != null)
                    userProp.SetValue(user, dtoProp.GetValue(dto));
            }
            
            var pwdSalt = _authSrv.GeneratePasswordHashSalt(dto.Password);
            user.Password = pwdSalt.Item1;
            user.Salt = pwdSalt.Item2;

            await _context.SaveChangesAsync();
            
            return Ok(user.FilterSerialize(authUser));
        }

        [HttpPut("{id}/Deposit")]
        public async Task<IActionResult> Deposit(int id, [FromBody] int amount = 0)
        {
            var authUser = await _authSrv.GetUser(User);

            if (authUser == null) return Unauthorized();
            if (authUser.Id != id && authUser.Role != UserRole.Administrator) return Forbid();

            if (amount < 0) return BadRequest();

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

        [HttpGet("{userId}/Notification")]
        public async Task<IActionResult> GetNotificationsForUID(
            int userId, 
            [FromQuery, Range(1, int.MaxValue)] int limit = 10, 
            [FromQuery, Range(1, int.MaxValue)] int page = 1
        )
        {
            var authUser = await _authSrv.GetUser(User);

            if (authUser == null) return Unauthorized();
            if (authUser.Id != userId && authUser.Role != UserRole.Administrator) return Forbid();

            return Ok(
                await _context.Notifications
                .Where(x => x.UserId == userId)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync()
            );
        }

        [HttpGet("{userId}/Notification/{notificationId}")]
        public async Task<IActionResult> GetNotificationByIdAndUID(int userId, int notificationId)
        {
            var authUser = await _authSrv.GetUser(User);

            if (authUser == null) return Unauthorized();
            if (authUser.Id != userId && authUser.Role != UserRole.Administrator) return Forbid();

            var message = await _context.Notifications.FirstOrDefaultAsync(x => x.UserId == userId && x.NotificationId == notificationId);
            if (message == null) return NotFound();

            return Ok(message);
        }

        [HttpPut("{userId}/Notification/{notificationId}")]
        public async Task<IActionResult> SetNotificationReadByUIDAndId(int userId, int notificationId, bool read = true)
        {
            var authUser = await _authSrv.GetUser(User);

            if (authUser == null) return Unauthorized();
            if (authUser.Id != userId && authUser.Role != UserRole.Administrator) return Forbid();

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.UserId == userId && x.NotificationId == notificationId);
            if (notification == null) return NotFound();

            notification.Read = read;
            await _context.SaveChangesAsync();

            return Ok(notification);
        }

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
