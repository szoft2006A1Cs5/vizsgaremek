using backend.Contexts;
using backend.DTOs.User;
using backend.Models;
using backend.Services;
using backend.Services.ResourceService;
using backend.VisibilityFiltering;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.RegularExpressions;

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
            var authUser = await _authSrv.GetUser(User, _context);

            var user = await _context.Users
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(x => x.Rentals)
                .ThenInclude(x => x.Vehicle)
                .ThenInclude(x => x.Owner)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.Images)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.Availabilities)
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.Rentals)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);
            
            if (user == null) return NotFound();

            return Ok(user.FilterSerialize(authUser));
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthUser()
        {
            var authUser = await _authSrv.GetUser(User, _context);

            if (authUser == null) return Unauthorized();

            return await GetUserById(authUser.Id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserById(int id, [FromBody] UserModificationDTO dto)
        {
            if (!dto.CheckValid())
                return BadRequest(new { Error = "A megadott adatok hibásak!" });
            
            var authUser = await _authSrv.GetUser(User, _context);

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

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserModificationDTO dto)
        {
            var authUID = _authSrv.GetUID(User);
            if (authUID == null) return Unauthorized();

            return await UpdateUserById(authUID.Value, dto);
        }

        [HttpPut("{id}/Image")]
        public async Task<IActionResult> UpdateUserImageById(int id, IFormFile? file)
        {
            var authUser = await _authSrv.GetUser(User, _context);

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

        [HttpPut("Image")]
        public async Task<IActionResult> UpdateUserImage(IFormFile? file)
        {
            var authUID = _authSrv.GetUID(User);
            if (authUID == null) return Unauthorized();

            return await UpdateUserImageById(authUID.Value, file);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            var authUID = _authSrv.GetUID(User);
            if (authUID == null) return Unauthorized();

            return await DeleteUserById(authUID.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserById(int id)
        {
            var authUser = await _authSrv.GetUser(User, _context);

            if (authUser == null) return Unauthorized();
            if (authUser.Id != id && authUser.Role != UserRole.Administrator) return Forbid();

            var user = await _context.Users
                .Include(x => x.Vehicles)
                .ThenInclude(x => x.Availabilities)
                .Include(x => x.Rentals)
                .Include(x => x.Notifications)
                .FirstOrDefaultAsync(x => x.Id == id);

            // Nem kene, hogy lehetseges legyen
            if (user == null) return NotFound();

            // TODO: jarmu elerhetosegek torlese, meg nem elfogadott berlesek
            //       torlese (es rendszerertesitesek kuldese a masik felnek),
            //       a mar elfogadott berlesek visszamondasa (kiveve nyilvan
            //       a befejezetteket)

            return NoContent();
        }
    }
}
