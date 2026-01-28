using backend.Auth;
using backend.Contexts;
using backend.Controllers;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NuGet.ContentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace backend.UnitTests.Tests
{
    [TestClass]
    public sealed class UserControllerTests
    {
        UserController? _controller;

        [TestInitialize]
        public void Initialize()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            AuthManager authMgr = new AuthManager(config);
            Context context = new Context(DbContextMock.Create());

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new Claim(JwtRegisteredClaimNames.Email, "tesztelek@teszt.hu"),
                        new Claim(JwtRegisteredClaimNames.Sub, "1"),
                        new Claim(JwtRegisteredClaimNames.Name, "1"),
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    ]
                )
            );

            _controller = new UserController(context, authMgr);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            context.Users.AddRange([
                new User
                {
                    Id = 1,
                    IdCardNumber = "123456AA",
                    Name = "Teszt Elek",
                    Email = "tesztelek@teszt.hu",
                    Phone = "36701234567",
                    Password = [],
                    Salt = [],
                    DriversLicenseNumber = "AA123456",
                    DriversLicenseDate = new DateTime(),
                    AddressZipcode = "1000",
                    AddressSettlement = "Budapest",
                    AddressStreetHouse = "Utca utca 1.",
                    Balance = 0,
                }
            ]);

            context.SaveChanges();
        }
        
        [TestMethod]
        public async Task GetUser1_Ok()
        {
            var result = (await _controller!.Get(1)) as ContentResult;
            Assert.IsNotNull(result);
        }
    }
}
