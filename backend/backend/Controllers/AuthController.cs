using backend.Auth;
using backend.Contexts;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    public class LoginCredentialsObject
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class RegistrationObject
    {
        public required string IdCardNumber { get; set; }
        public required string Name { get; set; }
        public required string Phone { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string DriversLicenseNumber { get; set; }
        public DateOnly DriversLicenseDate { get; set; }
        public required string AddressZipcode { get; set; }
        public required string AddressSettlement { get; set; }
        public required string AddressStreetHouse { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthManager _authMgr;
        private readonly Context _context;

        public AuthController(Context ctx, AuthManager authManager)
        {
            _context = ctx;
            _authMgr = authManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCredentialsObject credentials)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == credentials.Email);

            if (user == null)
                return Unauthorized();

            if (!_authMgr.VerifyPassword(credentials.Password, user))
                return Unauthorized();

            var jwt = _authMgr.GenerateJWT(user);
            return jwt != null ? Ok(new { UserId = user.Id, Token = jwt }) : StatusCode(500);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationObject registration)
        {
            var hashSalt = _authMgr.GeneratePasswordHashSalt(registration.Password);

            User user = new User {
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

            if (0 < _context.Users.Count(x => x.Email == user.Email) ||
                0 < _context.Users.Count(x => x.Phone == user.Phone) ||
                0 < _context.Users.Count(x => x.IdCardNumber == user.IdCardNumber) ||
                0 < _context.Users.Count(x => x.DriversLicenseNumber == user.DriversLicenseNumber))
                return StatusCode(409);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var jwt = _authMgr.GenerateJWT(user);
            return jwt != null ? Ok(new { UserId = user.Id, Token = jwt }) : StatusCode(500);
        }
    }
}
