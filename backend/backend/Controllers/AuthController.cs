using backend.Services;
using backend.Contexts;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.DTOs.Auth;
using backend.DTOs.User;
using backend.Services.EmailService;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authSrv;
        private readonly Context _context;
        private readonly IEmailService? _emailSrv;

        public AuthController(Context ctx, AuthService authSrv, IEmailService? emailSrv)
        {
            _context = ctx;
            _authSrv = authSrv;
            _emailSrv = emailSrv;
        }
        
        /// <summary>
        /// Bejelentkezés e-maillel és jelszóval
        /// </summary>
        /// <param name="credentials">E-mail cím és jelszó</param>
        /// <returns>JWT ha sikeres, másképpen 401-es HTTP kód</returns>
        [HttpPost("Login")]
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

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserDTO registration)
        {
            if (!registration.CheckValid())
                return BadRequest(new { Error = "A megadott adatok hibásak!" });

            var phone = registration.Phone.Substring(2);
            if (_context.Users.Any(x => x.Email == registration.Email ||
                                        x.Phone.EndsWith(phone) ||
                                        x.IdCardNumber == registration.IdCardNumber ||
                                        (registration.DriversLicenseNumber != null ? 
                                            x.DriversLicenseNumber == registration.DriversLicenseNumber :
                                            false)))
                return Conflict();
            
            var hashSalt = _authSrv.GeneratePasswordHashSalt(registration.Password);
            
            var user = new User {
                Name = registration.Name,
                Phone = registration.Phone,
                DateOfBirth = registration.DateOfBirth,
                Email = registration.Email,
                Password = hashSalt.Item1,
                Salt = hashSalt.Item2,
                IdCardNumber = registration.IdCardNumber,
                DriversLicenseNumber = registration.DriversLicenseNumber,
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

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (_emailSrv == null) return StatusCode(500);
            
            var emailUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());

            if (emailUser != null)
            {
                var token = await _authSrv.CreateToken(emailUser, TokenType.PasswordReset);
                await _emailSrv.SendPasswordResetEmailAsync(emailUser.Email, token);
            }
            
            return Ok();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(PasswordResetDTO dto)
        {
            var resetUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower() == dto.Email.ToLower());

            if (resetUser == null) return BadRequest();

            if (await _authSrv.VerifyToken(resetUser, dto.Token, TokenType.PasswordReset))
            {
                var (pass, salt) = _authSrv.GeneratePasswordHashSalt(dto.Password);
                
                resetUser.Password = pass;
                resetUser.Salt = salt;

                await _context.SaveChangesAsync();

                return Ok();
            }

            return BadRequest();
        }
    }
}
