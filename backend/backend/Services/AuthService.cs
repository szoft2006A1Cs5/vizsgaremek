using backend.Contexts;
using backend.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace backend.Services
{
    public class AuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration configuration)
        {
            _config = configuration;
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(
                password,
                salt,
                KeyDerivationPrf.HMACSHA512,
                10000,
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
                IssuedAt = DateTime.UtcNow,
                Issuer = iss,
                Audience = aud,
                Expires = DateTime.UtcNow.AddDays(7),
            };

            return new JsonWebTokenHandler().CreateToken(token);
        }

        public int? GetUID(ClaimsPrincipal claims)
        {
            var uidClaim = claims.FindFirst(ClaimTypes.NameIdentifier);

            if (uidClaim == null) return null;
            if (!int.TryParse(uidClaim.Value, out var uid)) return null;

            return uid;
        }

        public async Task<User?> GetUser(ClaimsPrincipal claims, Context context)
        {
            var uid = GetUID(claims);

            if (uid == null) return null;

            return await context.Users
                .Include(x => x.Rentals)
                .Include(x => x.Vehicles)
                .FirstOrDefaultAsync(x => x.Id == uid);
        }
    }
}
