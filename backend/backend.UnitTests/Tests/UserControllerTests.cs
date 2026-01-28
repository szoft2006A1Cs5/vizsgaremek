using backend.Auth;
using backend.Contexts;
using backend.Controllers;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NuGet.ContentModel;
using System.Security.Claims;
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

            var claims = new Claim[]
            {
            };

            _controller = new UserController(context, authMgr);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = 
                }
            };

            context.Users.AddRange(new User[]
            {
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
            });
        }
        
        [TestMethod]
        public async Task GetUser1_Ok()
        {
            var result = (await _controller!.Get(1)) as OkObjectResult;
            Assert.AreEqual("a", "a");
            Assert.IsNotNull(result);
        }
    }
}
