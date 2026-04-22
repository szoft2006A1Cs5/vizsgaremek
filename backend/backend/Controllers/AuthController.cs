using backend.Common;
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
        private readonly TimeProvider _timePrv;

        public AuthController(Context ctx, AuthService authSrv, TimeProvider timePrv, IEmailService? emailSrv = null)
        {
            _context = ctx;
            _authSrv = authSrv;
            _emailSrv = emailSrv;
            _timePrv = timePrv;
        }
        
        /// <summary>
        /// Bejelentkezés e-maillel és jelszóval
        /// </summary>
        /// <param name="credentials">E-mail cím és jelszó</param>
        /// <returns>JWT Cookie + UserId ha sikeres, másképpen 401-es HTTP kód</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO credentials)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == credentials.Email);

            if (user == null)
                return Unauthorized();

            if (!_authSrv.VerifyPassword(credentials.Password, user))
                return Unauthorized();
            
            return _authSrv.AddJWTCookie(user, Response) ? Ok(new { UserId = user.Id }) : StatusCode(500);
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("auth", new CookieOptions
            {
                HttpOnly = true,
                Secure = Config.CookieSecure,
                SameSite = Config.CookieSameSite,
            });
            
            return Ok();
        }

        /// <summary>
        /// Regisztral egy felhasznalot a rendszerbe
        /// </summary>
        /// <param name="registration">A regisztracios adatok</param>
        /// <returns>400-at ha hibas adatokat adtak meg,
        /// 409-et ha utkozik valamely mas felhasznalo adataival,
        /// 200-at + JWT Cookie + UserId, ha sikeres a regisztracio</returns>
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO registration)
        {
            if (!registration.CheckValid(_timePrv))
                return BadRequest(new { Error = "A megadott adatok formátumai hibásak!" });

            var phone = registration.Phone.Substring(2);
            if (_context.Users.Any(x => x.Email == registration.Email ||
                                        x.Phone.EndsWith(phone) ||
                                        x.IdCardNumber == registration.IdCardNumber ||
                                        (registration.DriversLicenseNumber != null ? 
                                            x.DriversLicenseNumber == registration.DriversLicenseNumber :
                                            false)))
                return Conflict();
            
            var (pass, salt) = _authSrv.GeneratePasswordHashSalt(registration.Password);
            
            var user = new User {
                Name = registration.Name,
                Phone = registration.Phone,
                DateOfBirth = registration.DateOfBirth,
                Email = registration.Email,
                Password = pass,
                Salt = salt,
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

            return _authSrv.AddJWTCookie(user, Response) ? Ok(new { UserId = user.Id }) : StatusCode(500);
        }

        /// <summary>
        /// Jelszo visszaallito emailt kuld a megadott email cimre,
        /// ha azzal letezik felhasznaloi fiok
        /// </summary>
        /// <param name="email">A felhasznalo email cime</param>
        /// <returns>
        /// 503-at ha nem elerheto az email kuldo szolgaltatas,
        /// 200-at minden mas esetben
        /// </returns>
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (_emailSrv == null)
                return StatusCode(503, 
                    new
                    {
                        Error = "Sajnos most nem tudtunk e-mailt küldeni! " +
                                $"Lépjen kapcsolatba velünk a {Config.SupportEmail} e-mail címen!"
                    }
                );
            
            var emailUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower());

            if (emailUser != null)
            {
                var token = await _authSrv.CreateToken(emailUser, TokenType.PasswordReset);
                await _emailSrv.SendPasswordResetEmailAsync(emailUser.Email, token);
            }
            
            return Ok();
        }

        /// <summary>
        /// Megvaltoztatja a felhasznalo jelszavat
        /// a kikuldott kod altal azonositva
        /// </summary>
        /// <param name="dto">A visszaallito adatok (email, visszaallito kod, uj jelszo)</param>
        /// <returns>
        /// 200-at ha sikeres,
        /// 400-at ha nem az.
        /// </returns>
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(PasswordResetDTO dto)
        {
            var resetUser = await _context.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower() == dto.Email.ToLower());

            const string badRequestError = "Hibás kód vagy e-mail cím!";
            if (resetUser == null) return BadRequest(new { Error = badRequestError });

            if (await _authSrv.VerifyToken(resetUser, dto.Token, TokenType.PasswordReset))
            {
                var (pass, salt) = _authSrv.GeneratePasswordHashSalt(dto.Password);
                
                resetUser.Password = pass;
                resetUser.Salt = salt;

                await _context.SaveChangesAsync();

                return Ok();
            }

            return BadRequest(new { Error = badRequestError });
        }
    }
}
