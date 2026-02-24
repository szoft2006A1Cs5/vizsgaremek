using backend.Services;
using backend.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using backend.DTOs.User;
using backend.Models;
using backend.VisibilityFiltering;
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

        public UserController(Context ctx, AuthService authSrv)
        {
            _context = ctx;
            _authSrv = authSrv;
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
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
                .AsSplitQuery()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            
            if (user == null) return NotFound();

            return Ok(user.FilterSerialize(authUser));
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthUser()
        {
            var authUser = await _authSrv.GetUser(User, _context);

            if (authUser == null) return Unauthorized();

            return await Get(authUser.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UserModificationDTO dto)
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
            
            if (_context.Users.Any(x => x.Email == dto.Email) ||
                _context.Users.Any(x => x.Phone == dto.Phone) ||
                _context.Users.Any(x => x.IdCardNumber == dto.IdCardNumber) ||
                _context.Users.Any(x => x.DriversLicenseNumber == dto.DriversLicenseNumber))
                return StatusCode(409);
            
            var authUser = await _authSrv.GetUser(User, _context);

            if (authUser == null) return Unauthorized();
            if (authUser.Role != UserRole.Administrator &&
                !_authSrv.VerifyPassword(dto.PreviousPassword, authUser)) return Forbid();

            var userProps = typeof(User).GetProperties();
            foreach (var dtoProp in typeof(UserModificationDTO).GetProperties())
            {
                var userProp =
                    userProps.FirstOrDefault(x => x.Name == dtoProp.Name &&
                                                  x.PropertyType == dtoProp.PropertyType);
                
                if (userProp != null)
                    userProp.SetValue(authUser, dtoProp.GetValue(dto));
            }
            
            var pwdSalt = _authSrv.GeneratePasswordHashSalt(dto.Password);
            authUser.Password = pwdSalt.Item1;
            authUser.Salt = pwdSalt.Item2;

            await _context.SaveChangesAsync();
            
            return Ok(authUser.FilterSerialize(authUser));
        }
    }
}
