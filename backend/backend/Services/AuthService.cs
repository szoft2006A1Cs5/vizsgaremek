using backend.Contexts;
using backend.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using backend.Common;

namespace backend.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;
        private readonly Context _context;
        private readonly TimeProvider _timePrv;

        public AuthService(
            IConfiguration configuration, 
            Context context, 
            TimeProvider timePrv
        )
        {
            _config = configuration;
            _context = context;
            _timePrv = timePrv;
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA512,
                100000,
                64
            );
        }

        public bool VerifyPassword(string password, User user)
        {
            return CryptographicOperations.FixedTimeEquals(HashPassword(password, user.Salt), user.Password);
        }

        public Tuple<byte[], byte[]> GeneratePasswordHashSalt(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = HashPassword(password, salt);

            return Tuple.Create(hash, salt);
        }

        public string? GenerateJWT(User user)
        {
            var key = _config["Auth:Jwt:Secret"];
            var iss = _config["Auth:Issuer"];
            var aud = _config["Auth:Audience"];

            if (key == null || iss == null || aud == null)
                return null;

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCreds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, $"{user.Id}"),
                    new Claim(JwtRegisteredClaimNames.Name, $"{user.Id}"),
                    new Claim(ClaimTypes.NameIdentifier, $"{user.Id}"),
                    new Claim(ClaimTypes.Role, $"{user.Role}"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                ]),
                SigningCredentials = signingCreds,
                IssuedAt = _timePrv.GetUtcNow().UtcDateTime,
                Issuer = iss,
                Audience = aud,
                Expires = _timePrv.GetUtcNow().UtcDateTime.AddHours(Config.CookieExpirationHours),
            };

            return new JsonWebTokenHandler().CreateToken(token);
        }

        public bool AddJWTCookie(User user, HttpResponse response)
        {
            var jwt = GenerateJWT(user);
            if (jwt == null) return false;
            
            response.Cookies.Append("auth", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = Config.CookieSecure,
                SameSite = Config.CookieSameSite,
                Expires = _timePrv.GetUtcNow().UtcDateTime.AddHours(Config.CookieExpirationHours),
            });

            return true;
        }

        public int? GetUID(ClaimsPrincipal claims)
        {
            var uidClaim = claims.FindFirst(ClaimTypes.NameIdentifier);

            if (uidClaim == null) return null;
            if (!int.TryParse(uidClaim.Value, out var uid)) return null;

            return uid;
        }

        public async Task<User?> GetUser(ClaimsPrincipal claims)
        {
            var uid = GetUID(claims);

            if (uid == null) return null;

            return await _context.Users
                .Include(x => x.Rentals)
                .Include(x => x.Vehicles)
                .FirstOrDefaultAsync(x => x.Id == uid);
        }

        public async Task<string> CreateToken(User user, TokenType type)
        {
            var token = RandomNumberGenerator.GetInt32(100_000, 1_000_000).ToString();

            await _context.UserTokens.AddAsync(new UserToken
            {
                UserId = user.Id,
                User = user,
                Token = token,
                Type = type,
                TimeCreated = _timePrv.GetUtcNow().UtcDateTime,
            });
            await _context.SaveChangesAsync();

            return token;
        }

        public async Task<bool> VerifyToken(User user, string token, TokenType type)
        {
            var userToken = await _context.UserTokens
                .FirstOrDefaultAsync(x => x.UserId == user.Id && x.Token == token && x.Type == type);
            
            if (userToken == null) return false;
            _context.UserTokens.Remove(userToken);
            await _context.SaveChangesAsync();

            return _timePrv.GetUtcNow().UtcDateTime <= userToken.TimeCreated.AddMinutes(Config.TokenValidMins);
        }
    }
}
