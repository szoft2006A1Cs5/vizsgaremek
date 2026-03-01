using backend.Services;
using backend.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using backend.DTOs.User;
using backend.Models;
using backend.VisibilityFiltering;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Context _context;
        private readonly AuthService _authSrv;

        public UserController(Context ctx, AuthService authSrv)
        {
            _context = ctx;
            _authSrv = authSrv;
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
            if (!Regex.IsMatch(dto.Name,
                    @"^[A-ZÁÉÍÓÚÜŰÖŐ][a-záéíóúüűöő]+( [A-ZÁÉÍÓÚÜŰÖŐ][a-záéíóúüűöő]+)+$") ||
                !Regex.IsMatch(dto.IdCardNumber, @"^\d{6}[A-Z]{2}$") ||
                !Regex.IsMatch(dto.DriversLicenseNumber, @"^[A-Z]{2}\d{6}$") ||
                !Regex.IsMatch(dto.Email, @"^[A-z0-9.-]+@([A-z0-9-]+\.)+(com|hu)$") ||
                !Regex.IsMatch(dto.Phone, @"^(36|06)(94|70|30|20)\d{7}$") ||
                !Regex.IsMatch(dto.AddressZipcode, @"^\d{4}$") ||
                !(dto.DateOfBirth.ToDateTime(new TimeOnly(0)).AddYears(18) <= DateTime.Now))
                return BadRequest(new { Error = "A megadott adatok hibásak!" });
            
            var authUser = await _authSrv.GetUser(User, _context);

            if (authUser == null) return Unauthorized(); 
            if (authUser.Id != id && authUser.Role != UserRole.Administrator) return Forbid();

            // Duplan nezzuk az authUser id-t, de megeri, mert igy atugorhatunk egy adatbazis lekerdezest.
            var user = authUser.Id == id ? authUser : await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return NotFound();
            
            if (authUser.Role != UserRole.Administrator &&
                !_authSrv.VerifyPassword(dto.PreviousPassword, user)) return Forbid();

            if (_context.Users.Any(x => x.Email == dto.Email && x != user) ||
                _context.Users.Any(x => x.Phone.Substring(2) == dto.Phone.Substring(2) && x != user) ||
                _context.Users.Any(x => x.IdCardNumber == dto.IdCardNumber && x != user) ||
                _context.Users.Any(x => x.DriversLicenseNumber == dto.DriversLicenseNumber && x != user))
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
        
        // TODO: Profilkep hozzaadasa IResourceService-el
        //       Kerdeses, mert ha IFormFile-t megadok parameterkent,
        //       akkor az egesz input json-rol formdata-va valtozik,
        //       ami nem konzisztens a tobbi metodussal
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserModificationDTO dto)
        {
            var authUID = _authSrv.GetUID(User);
            if (authUID == null) return Unauthorized();

            return await UpdateUserById(authUID.Value, dto);
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
