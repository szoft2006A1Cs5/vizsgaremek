using backend.Auth;
using backend.Contexts;
using backend.Controllers;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace backend.UnitTests
{
    public struct TestingEnvironment
    {
        public IConfiguration Configuration { get; set; }
        public Context Context { get; set; }
        public AuthManager AuthManager { get; set; }
    };

    internal static class MockContext
    {
        public static TestingEnvironment CreateContext()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            AuthManager authMgr = new AuthManager(config);
            Context context = new Context(GetOptions());

            AddData(context);

            return new TestingEnvironment { 
                Configuration = config,
                Context = context,
                AuthManager = authMgr,
            };
        }

        public static void AddData(Context context)
        {
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

        public static ClaimsPrincipal GetClaimsPrincipalFor(int id, UserRole role)
        {
            return new ClaimsPrincipal(
                new ClaimsIdentity(
                    [
                        new Claim(JwtRegisteredClaimNames.Sub, $"{id}"),
                        new Claim(ClaimTypes.NameIdentifier, $"{id}"),
                        new Claim(ClaimTypes.Role, $"{role}"),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    ],
                    "Custom"
                )
            );
        }

        public static DbContextOptions<Context> GetOptions()
        {
            var options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return options;
        }
    }
}
