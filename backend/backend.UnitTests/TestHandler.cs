using backend.Services;
using backend.Contexts;
using backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Data.Sqlite;

namespace backend.UnitTests
{
    public struct TestingEnvironment
    {
        public IConfiguration Configuration { get; set; }
        public Context Context { get; set; }
        public AuthService AuthService { get; set; }
        public MockResourceService ResourceService { get; set; }
    };

    internal static class TestHandler
    {
        public static void SetAuthUser<T>(this T? controller, int? uid, UserRole? role) where T : ControllerBase
        {
            if (controller == null) return;
            
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = uid != null && role != null ?
                        GetClaimsPrincipalFor(uid.Value, role.Value) : 
                        new ClaimsPrincipal() 
                }
            };
        }
        
        public static TestingEnvironment CreateEnvironment()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            AuthService authSrv = new AuthService(config);
            Context context = new Context(GetOptions());
            MockResourceService resSrv = new MockResourceService();
            context.Database.EnsureCreated();

            AddData(context);

            return new TestingEnvironment { 
                Configuration = config,
                Context = context,
                AuthService = authSrv,
                ResourceService = resSrv
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
                    Phone = "36201234567",
                    DateOfBirth = new DateTime(2004, 04, 18),
                    Password = Convert.FromHexString("5cd79118803c295ee4566a87a59423e7b4b020194520f52e78bddcdbfb36daef43b032e7a122323e5849344ff1fd625a7885c3ff62688a0241b7e4018ed3d9e0"),
                    Salt = Convert.FromHexString("945200f84cef838d8d44e0121415fa53"),
                    DriversLicenseNumber = "AA123456",
                    DriversLicenseDate = new DateTime(),
                    AddressZipcode = "9700",
                    AddressSettlement = "Szombathely",
                    AddressStreetHouse = "Zrínyi Ilona utca 12.",
                    Balance = 0,
                    ProfilePicPath = null,
                    Role = UserRole.User
                },
                new User
                {
                    Id = 2,
                    IdCardNumber = "123456BB",
                    Name = "Gipsz Jakab",
                    Email = "gipszjakab@teszt.hu",
                    Phone = "36701234567",
                    DateOfBirth = new DateTime(1995, 07, 21),
                    Password = [],
                    Salt = [],
                    DriversLicenseNumber = "BB123456",
                    DriversLicenseDate = new DateTime(),
                    AddressZipcode = "1117",
                    AddressSettlement = "Budapest",
                    AddressStreetHouse = "Budafoki út 12.",
                    Balance = 0,
                    ProfilePicPath = null,
                    Role = UserRole.User
                },
                new User
                {
                    Id = 3,
                    IdCardNumber = "123456CC",
                    Name = "Vincs Eszter",
                    Email = "vincseszter@teszt.hu",
                    Phone = "36301234567",
                    DateOfBirth = new DateTime(2000, 11, 12),
                    Password = [],
                    Salt = [],
                    DriversLicenseNumber = "CC123456",
                    DriversLicenseDate = new DateTime(),
                    AddressZipcode = "9700",
                    AddressSettlement = "Szombathely",
                    AddressStreetHouse = "Kéthly Anna utca 7.",
                    Balance = 0,
                    ProfilePicPath = null,
                    Role = UserRole.User
                },
                new User
                {
                    Id = 4,
                    IdCardNumber = "123456DD",
                    Name = "Admin Tamás",
                    Email = "admintamas@comove.hu",
                    Phone = "36941234567",
                    DateOfBirth = new DateTime(1985, 12, 11),
                    Password = [],
                    Salt = [],
                    DriversLicenseNumber = "DD123456",
                    DriversLicenseDate = new DateTime(),
                    AddressZipcode = "9700",
                    AddressSettlement = "Szombathely",
                    AddressStreetHouse = "Zrínyi Ilona utca 12.",
                    Balance = 0,
                    ProfilePicPath = null,
                    Role = UserRole.Administrator
                }
            ]);
            
            context.Vehicles.AddRange([
                new Vehicle
                {
                    Id = 1,
                    OwnerId = 1,
                    Availabilities = [],
                    Images = [],
                    Rentals = [],
                    AvgFuelConsumption = 6.2,
                    Description = "",
                    LicensePlate = "ABC123",
                    InsuranceNumber = "KGFB12345678",
                    Manufacturer = "Toyota",
                    Model = "Corolla",
                    Year = 2019,
                    OdometerReading = 12000,
                    VIN = "ABCDEF123ARS12ABC1",
                },
                new Vehicle
                {
                    Id = 2,
                    OwnerId = 2,
                    Availabilities = [],
                    Images = [],
                    Rentals = [],
                    AvgFuelConsumption = 5.1,
                    Description = "",
                    LicensePlate = "DEF456",
                    InsuranceNumber = "KGFB87654321",
                    Manufacturer = "Suzuki",
                    Model = "Swift",
                    Year = 2010,
                    OdometerReading = 72000,
                    VIN = "12ARTC2131KBEBU234",
                },
            ]);

            context.VehicleAvailabilities.AddRange([
                new VehicleAvailability
                {
                    Id = 1,
                    AvailabilityId = 1,
                    VehicleId = 1,
                    Start = new DateTime(2026, 02, 27),
                    End = new DateTime(2026, 03, 18),
                    HourlyRate = 600,
                },
                new VehicleAvailability
                {
                    Id = 2,
                    AvailabilityId = 1,
                    VehicleId = 2,
                    Start = new DateTime(2026, 01, 13),
                    End = new DateTime(2026, 04, 25),
                    HourlyRate = 400,
                },
                new VehicleAvailability
                {
                    Id = 3,
                    AvailabilityId = 2,
                    VehicleId = 1,
                    Start = new DateTime(2026, 01, 13),
                    End = new DateTime(2026, 02, 12),
                }
            ]);
            
            context.VehicleImages.AddRange([
                new VehicleImage
                {
                    Id = 1,
                    ImageId = 1,
                    VehicleId = 1,
                    Path = "res/toyotacorollaimage1.jpg",
                    SortIndex = 1,
                },
                new VehicleImage
                {
                    Id = 2,
                    ImageId = 2,
                    VehicleId = 1,
                    Path = "res/toyotacorollaimage2.jpg",
                    SortIndex = 2,
                },
                new VehicleImage
                {
                    Id = 3,
                    ImageId = 1,
                    VehicleId = 2,
                    Path = "res/suzukiswiftimage1.jpg",
                    SortIndex = 1,
                }
            ]);
            
            context.Rentals.AddRange([
                new Rental
                {
                    Id = 1,
                    VehicleId = 1,
                    RenterId = 2,
                    Start = new DateTime(2026, 02, 28, 10, 00, 00),
                    End = new DateTime(2026, 03, 01, 15, 00, 00),
                    Status = RentalStatus.Finished,
                    FullPrice = 29 * 600,
                    Downpayment = (int)(29 * 600 * 0.3),
                    FuelLevel = 50,
                    PickupLatitude = 47.228641,
                    PickupLongtitude = 16.624567,
                    OwnerRating = 5.0,
                    RenterRating = 4.5
                },
                new Rental
                {
                    Id = 2,
                    VehicleId = 2,
                    RenterId = 3,
                    Start = new DateTime(2026, 03, 11, 12, 00, 00),
                    End = new DateTime(2026, 03, 19, 15, 00, 00),
                    Status = RentalStatus.OfferAccepted,
                    FullPrice = 195 * 400,
                    Downpayment = (int)(195 * 400 * 0.3),
                    FuelLevel = 45,
                    PickupLatitude = 47.228641,
                    PickupLongtitude = 16.624567,
                    OwnerRating = null,
                    RenterRating = null
                }
            ]);

            context.Notifications.AddRange([
                new Notification
                {
                    Id = 1,
                    Content = "A bérlési kérelmed elutasították és így törölve lett!",
                    Read = false,
                    TimeSent = new DateTime(2026, 03, 02, 11, 12, 23),
                    UserId = 1
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
            var conn = new SqliteConnection("DataSource=:memory:");
            conn.Open();
            var options = new DbContextOptionsBuilder<Context>()
                .UseSqlite(conn)
                .Options;

            return options;
        }
    }
}
