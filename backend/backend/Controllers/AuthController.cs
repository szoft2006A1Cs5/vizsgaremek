using backend.Services;
using backend.Contexts;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using backend.DTOs.Auth;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authSrv;
        private readonly Context _context;

        public AuthController(Context ctx, AuthService authSrv)
        {
            _context = ctx;
            _authSrv = authSrv;
        }

        /// <summary>
        /// Bejelentkezés e-maillel és jelszóval
        /// </summary>
        /// <param name="credentials">E-mail cím és jelszó</param>
        /// <returns>JWT ha sikeres, másképpen 401-es HTTP kód</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO credentials)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == credentials.Email);

            if (user == null)
                return Unauthorized();

            if (!_authSrv.VerifyPassword(credentials.Password, user))
                return Unauthorized();

            var jwt = _authSrv.GenerateJWT(user);
            return jwt != null ? Ok(new { UserId = user.Id, Token = jwt }) : StatusCode(500);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationDTO registration)
        {
            if (!Regex.IsMatch(registration.Name, @"^[A-ZÁÉÍÓÚÜŰÖŐ][a-záéíóúüűöő]+( [A-ZÁÉÍÓÚÜŰÖŐ][a-záéíóúüűöő]+)+$") ||
                !Regex.IsMatch(registration.IdCardNumber, @"^\d{6}[A-Z]{2}$") ||
                !Regex.IsMatch(registration.DriversLicenseNumber, @"^[A-Z]{2}\d{6}$") ||
                !Regex.IsMatch(registration.Email, @"^[A-z0-9.-]+@([A-z0-9-]+\.)+(com|hu)$") ||
                !Regex.IsMatch(registration.Phone, @"^(36|06)(94|70|30|20)\d{7}$") ||
                !Regex.IsMatch(registration.AddressZipcode, @"^\d{4}$") ||
                !(registration.DateOfBirth.ToDateTime(new TimeOnly(0)).AddYears(18) <= DateTime.Now))
                return BadRequest(new { Error = "A megadott adatok hibásak!" });
            
            if (_context.Users.Any(x => x.Email == registration.Email) ||
                _context.Users.Any(x => x.Phone == registration.Phone) ||
                _context.Users.Any(x => x.IdCardNumber == registration.IdCardNumber) ||
                _context.Users.Any(x => x.DriversLicenseNumber == registration.DriversLicenseNumber))
                return Conflict();
            
            var hashSalt = _authSrv.GeneratePasswordHashSalt(registration.Password);
            
            var user = new User {
                Name = registration.Name,
                Phone = registration.Phone,
                DateOfBirth = registration.DateOfBirth.ToDateTime(new TimeOnly(0)),
                Email = registration.Email,
                Password = hashSalt.Item1,
                Salt = hashSalt.Item2,
                IdCardNumber = registration.IdCardNumber,
                DriversLicenseNumber = registration.DriversLicenseNumber,
                DriversLicenseDate = registration.DriversLicenseDate.ToDateTime(new TimeOnly(0)),
                Role = UserRole.User,
                ProfilePicPath = null,
                AddressZipcode = registration.AddressZipcode,
                AddressSettlement = registration.AddressSettlement,
                AddressStreetHouse = registration.AddressStreetHouse,
                Balance = 0,
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var jwt = _authSrv.GenerateJWT(user);
            return jwt != null ? Ok(new { UserId = user.Id, Token = jwt }) : StatusCode(500);
        }
    }
}
