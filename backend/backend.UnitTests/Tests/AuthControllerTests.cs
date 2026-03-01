using backend.Controllers;
using backend.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace backend.UnitTests.Tests;

[TestClass]
public class AuthControllerTests
{
    TestingEnvironment _environment;
    private AuthController? _controller;
    
    [TestInitialize]
    public void Initialize()
    {
        _environment = TestHandler.CreateEnvironment();
        _controller = new AuthController(_environment.Context, _environment.AuthService);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _environment.Context.Dispose();
    }

    [TestMethod]
    public async Task PostLogin_Ok()
    {
        _controller.SetAuthUser(null, null);

        var result = (await _controller!.Login(new LoginDTO
        {
            Email = "tesztelek@teszt.hu",
            Password = "NagyTesztElek32",
        })) as OkObjectResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task PostLogin_Unauthorized()
    {
        _controller.SetAuthUser(null, null);
        
        var result = (await _controller!.Login(new LoginDTO
        {
            Email = "tesztelek@teszt.hu",
            Password = "RosszJelszo",
        })) as UnauthorizedResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task PostRegister_Ok()
    {
        _controller.SetAuthUser(null, null);
        
        var result = (await _controller!.Register(new RegistrationDTO
        {
            AddressStreetHouse = "Utca utca 1.",
            AddressZipcode = "9700",
            AddressSettlement = "Szombathely",
            DateOfBirth = new DateOnly(2003, 03, 12),
            DriversLicenseNumber = "FF123456",
            DriversLicenseDate = new DateOnly(),
            Email = "tesztpeter@teszt.hu",
            IdCardNumber = "123456FF",
            Name = "Teszt Péter",
            Password = "TesztPeterVok1!",
            Phone = "06709876543"
        })) as OkObjectResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task PostRegister_BadRequest()
    {
        _controller.SetAuthUser(null, null);
        
        var result = (await _controller!.Register(new RegistrationDTO
        {
            AddressStreetHouse = "Utca utca 1.",
            AddressZipcode = "9700",
            AddressSettlement = "Szombathely",
            DateOfBirth = new DateOnly(2003, 03, 12),
            DriversLicenseNumber = "FF123456",
            DriversLicenseDate = new DateOnly(),
            Email = "tesztpeter@teszt.", // Rossz az email formatuma
            IdCardNumber = "123456FF",
            Name = "Teszt Péter",
            Password = "TesztPeterVok1!",
            Phone = "06709876543"
        })) as BadRequestObjectResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task PostRegister_Conflict()
    {
        _controller.SetAuthUser(null, null);
        
        var result = (await _controller!.Register(new RegistrationDTO
        {
            AddressStreetHouse = "Utca utca 1.",
            AddressZipcode = "9700",
            AddressSettlement = "Szombathely",
            DateOfBirth = new DateOnly(2003, 03, 12),
            DriversLicenseNumber = "FF123456",
            DriversLicenseDate = new DateOnly(),
            Email = "tesztpeter@teszt.hu",
            IdCardNumber = "123456FF",
            Name = "Teszt Péter",
            Password = "TesztPeterVok1!",
            Phone = "06701234567" // Mar van ilyen telefonszammal user
        })) as ConflictResult;
        
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task TestAll_Ok()
    {
        _controller.SetAuthUser(null, null);
        
        #region Register
        var registerResult = (await _controller!.Register(new RegistrationDTO
        {
            AddressStreetHouse = "Utca utca 1.",
            AddressZipcode = "9700",
            AddressSettlement = "Szombathely",
            DateOfBirth = new DateOnly(2003, 03, 12),
            DriversLicenseNumber = "FF123456",
            DriversLicenseDate = new DateOnly(),
            Email = "tesztpeter@teszt.hu",
            IdCardNumber = "123456FF",
            Name = "Teszt Péter",
            Password = "TesztPeterVok1!",
            Phone = "06709876543"
        })) as OkObjectResult;
        
        Assert.IsNotNull(registerResult);
        #endregion
        
        #region Login
        var loginResult = (await _controller!.Login(new LoginDTO
        {
            Email = "tesztpeter@teszt.hu",
            Password = "TesztPeterVok1!",
        })) as OkObjectResult;
        
        Assert.IsNotNull(loginResult);
        #endregion
    }
}
