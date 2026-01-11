using backend.Contexts;
using backend.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace backend.Auth
{
    public class AuthManager
    {
        private readonly IConfiguration _config;

        public AuthManager(IConfiguration configuration)
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

        public string GenerateJWT(User user)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
            var signingCreds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, $"{user.Id}"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                ]),
                SigningCredentials = signingCreds,
                IssuedAt = DateTime.UtcNow,
                Issuer = "comove",
                Audience = "comoveUsers",
                Expires = DateTime.UtcNow.AddDays(7),
            };

            return new JsonWebTokenHandler().CreateToken(token);
        }
    }
}
