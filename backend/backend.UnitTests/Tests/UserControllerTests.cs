using backend.Services;
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
using backend.DTOs.User;

namespace backend.UnitTests.Tests
{
    [TestClass]
    public sealed class UserControllerTests
    {
        TestingEnvironment _environment;
        UserController? _controller;

        [TestInitialize]
        public void Initialize()
        {
            _environment = TestHandler.CreateEnvironment();
            _controller = new UserController(_environment.Context, _environment.AuthService);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _environment.Context.Dispose();
        }
        
        [TestMethod]
        public async Task GetUserPublic_Ok()
        {
            _controller.SetAuthUser(null, null);
            
            var result = (await _controller!.GetUserById(1)) as OkObjectResult;
            Assert.IsNotNull(result);
            var user = JsonConvert.DeserializeObject<User>((string)result.Value);
            Assert.IsNotNull(user);
            
            Assert.AreEqual(user.Id, 1);
            Assert.IsTrue(user.Phone == null); // In Relation
            Assert.IsNull(user.IdCardNumber); // Owner Only
            Assert.IsNull(user.Password); // Admin Only
        }

        [TestMethod]
        public async Task GetUser_NotFound()
        {
            _controller.SetAuthUser(1, UserRole.User);
            
            var result = (await _controller!.GetUserById(5)) as NotFoundResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetUserOwner_Ok()
        {
            _controller.SetAuthUser(1, UserRole.User);
            
            var result = (await _controller!.GetUserById(1)) as OkObjectResult;
            Assert.IsNotNull(result);
            var user = JsonConvert.DeserializeObject<User>((string)result.Value);
            Assert.IsNotNull(user);
         
            Assert.AreEqual(user.Id, 1);
            Assert.AreEqual("36201234567", user.Phone); // In Relation
            Assert.AreEqual("123456AA", user.IdCardNumber); // Owner Only
            Assert.IsNull(user.Password); // Admin Only
        }

        [TestMethod]
        public async Task GetUserInRelation_Ok()
        {
            _controller.SetAuthUser(1, UserRole.User);
            
            var result = (await _controller!.GetUserById(2)) as OkObjectResult;
            Assert.IsNotNull(result);
            var user = JsonConvert.DeserializeObject<User>((string)result.Value);
            Assert.IsNotNull(user);
         
            Assert.AreEqual(user.Id, 2);
            Assert.AreEqual("36701234567", user.Phone); // In Relation
            Assert.IsNull(user.IdCardNumber); // Owner Only
            Assert.IsNull(user.Password); // Admin Only
        }

        [TestMethod]
        public async Task GetUserAdmin_Ok()
        {
            _controller.SetAuthUser(4, UserRole.Administrator);
            
            var result = (await _controller!.GetUserById(1)) as OkObjectResult;
            Assert.IsNotNull(result);
            var user = JsonConvert.DeserializeObject<User>((string)result.Value);
            Assert.IsNotNull(user);
         
            Assert.AreEqual(user.Id, 1);
            Assert.AreEqual("36201234567", user.Phone); // In Relation
            Assert.IsNotNull(user.IdCardNumber); // Owner Only
            Assert.IsNotNull(user.Password); // Admin Only
        }

        [TestMethod]
        public async Task PutUser_Ok()
        {
            _controller.SetAuthUser(1, UserRole.User);

            var result = (await _controller!.UpdateUser(new UserModificationDTO
            {
                PreviousPassword = "NagyTesztElek32",
                IdCardNumber = "123456AA",
                Name = "Teszt Elek Péter",
                Phone = "36201234567",
                Email = "tesztelek@teszt.hu",
                Password = "NagyTesztElek32",
                DriversLicenseNumber = "AA123456",
                DriversLicenseDate = new DateOnly(),
                AddressStreetHouse = "Zrínyi Ilona utca 12.",
                AddressSettlement = "Szombathely",
                AddressZipcode = "9700"
            })) as OkObjectResult;
            
            Assert.IsNotNull(result);
            var user = JsonConvert.DeserializeObject<User>((string)result.Value);
            Assert.IsNotNull(user);
            
            Assert.AreEqual("Teszt Elek Péter", user.Name);
        }

        [TestMethod]
        public async Task PutUser_Unauthorized()
        {
            // Nem vagyunk bejelentkezve
            _controller.SetAuthUser(null, null);
            
            var result = (await _controller!.UpdateUser(new UserModificationDTO
            {
                PreviousPassword = "NagyTesztElek32",
                IdCardNumber = "123456AA",
                Name = "Teszt Elek Péter",
                Phone = "36201234567",
                Email = "tesztelek@teszt.hu",
                Password = "NagyTesztElek32",
                DriversLicenseNumber = "AA123456",
                DriversLicenseDate = new DateOnly(),
                AddressStreetHouse = "Zrínyi Ilona utca 12.",
                AddressSettlement = "Szombathely",
                AddressZipcode = "9700"
            })) as UnauthorizedResult;
            
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task PutUser_Forbidden()
        {
            _controller.SetAuthUser(1, UserRole.User);
            
            var result = (await _controller!.UpdateUser(new UserModificationDTO
            {
                PreviousPassword = "ElirtJelszo123",
                IdCardNumber = "123456AA",
                Name = "Teszt Elek Péter",
                Phone = "36201234567",
                Email = "tesztelek@teszt.hu",
                Password = "NagyTesztElek32",
                DriversLicenseNumber = "AA123456",
                DriversLicenseDate = new DateOnly(),
                AddressStreetHouse = "Zrínyi Ilona utca 12.",
                AddressSettlement = "Szombathely",
                AddressZipcode = "9700"
            })) as ForbidResult;
            
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task PutUser_BadRequest()
        {
            _controller.SetAuthUser(1, UserRole.User);
            
            var result = (await _controller!.UpdateUser(new UserModificationDTO
            {
                PreviousPassword = "NagyTesztElek32",
                IdCardNumber = "12345", // Helytelen szemelyi formatum
                Name = "Teszt Elek Péter",
                Phone = "36201234567",
                Email = "tesztelek@teszt.hu",
                Password = "NagyTesztElek32",
                DriversLicenseNumber = "AA123456",
                DriversLicenseDate = new DateOnly(),
                AddressStreetHouse = "Zrínyi Ilona utca 12.",
                AddressSettlement = "Szombathely",
                AddressZipcode = "9700"
            })) as BadRequestObjectResult;
            
            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public async Task PutUser_Conflict()
        {
            _controller.SetAuthUser(1, UserRole.User);
            
            var result = (await _controller!.UpdateUser(new UserModificationDTO
            {
                PreviousPassword = "NagyTesztElek32",
                IdCardNumber = "123456BB", // Ilyen szemelyivel mar van user
                Name = "Teszt Elek Péter",
                Phone = "36201234567",
                Email = "tesztelek@teszt.hu",
                Password = "NagyTesztElek32",
                DriversLicenseNumber = "AA123456",
                DriversLicenseDate = new DateOnly(),
                AddressStreetHouse = "Zrínyi Ilona utca 12.",
                AddressSettlement = "Szombathely",
                AddressZipcode = "9700"
            })) as ConflictResult;
            
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task PutUserByIdAdmin_Ok()
        {
            _controller.SetAuthUser(4, UserRole.Administrator);
            
            var result = (await _controller!.UpdateUserById(1, new UserModificationDTO
            {
                PreviousPassword = "",
                IdCardNumber = "123456AA",
                Name = "Teszt Elek Péter",
                Phone = "36201234567",
                Email = "tesztelek@teszt.hu",
                Password = "NagyTesztElek32",
                DriversLicenseNumber = "AA123456",
                DriversLicenseDate = new DateOnly(),
                AddressStreetHouse = "Zrínyi Ilona utca 12.",
                AddressSettlement = "Szombathely",
                AddressZipcode = "9700"
            })) as OkObjectResult;
            
            Assert.IsNotNull(result);
            var user = JsonConvert.DeserializeObject<User>((string)result.Value);
            Assert.IsNotNull(user);
            
            Assert.AreEqual("Teszt Elek Péter", user.Name);
        }

        [TestMethod]
        public async Task PutUserByIdAdmin_NotFound()
        {
            _controller.SetAuthUser(4, UserRole.Administrator);
            
            var result = (await _controller!.UpdateUserById(5, new UserModificationDTO
            {
                PreviousPassword = "",
                IdCardNumber = "123456AA",
                Name = "Teszt Elek Péter",
                Phone = "36201234567",
                Email = "tesztelek@teszt.hu",
                Password = "NagyTesztElek32",
                DriversLicenseNumber = "AA123456",
                DriversLicenseDate = new DateOnly(),
                AddressStreetHouse = "Zrínyi Ilona utca 12.",
                AddressSettlement = "Szombathely",
                AddressZipcode = "9700"
            })) as NotFoundResult;
            
            Assert.IsNotNull(result);
        }
        
        /*
        [TestMethod]
        public async Task DeleteUser_NoContent() {}
        */

        [TestMethod]
        public async Task DeleteUser_Unauthorized()
        {
            _controller.SetAuthUser(null, null);
            var result = await _controller!.DeleteUser() as UnauthorizedResult;
            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public async Task DeleteUserByIdAdmin_NotFound()
        {
            _controller.SetAuthUser(4, UserRole.Administrator);
            var result = await _controller!.DeleteUserById(5) as NotFoundResult;
            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public async Task DeleteUserById_Forbidden()
        {
            _controller.SetAuthUser(1, UserRole.User);
            var result = await _controller!.DeleteUserById(2) as ForbidResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task TestAll_Ok()
        {
            #region GetPublic
            _controller.SetAuthUser(null, null);
            
            var getPublicResult = (await _controller!.GetUserById(1)) as OkObjectResult;
            Assert.IsNotNull(getPublicResult);
            var publicUser = JsonConvert.DeserializeObject<User>((string)getPublicResult.Value);
            Assert.IsNotNull(publicUser);
            
            Assert.AreEqual(1, publicUser.Id);
            Assert.IsTrue(publicUser.Phone == null); // In Relation
            Assert.IsNull(publicUser.IdCardNumber); // Owner Only
            Assert.IsNull(publicUser.Password); // Admin Only
            #endregion
            
            Assert.AreEqual("Teszt Elek", publicUser.Name);
            
            #region Put
            _controller.SetAuthUser(1, UserRole.User);
            
            var putResult = (await _controller!.UpdateUser(new UserModificationDTO
            {
                PreviousPassword = "NagyTesztElek32",
                IdCardNumber = "123456AA",
                Name = "Teszt Elek Péter",
                Phone = "36201234567",
                Email = "tesztelek@teszt.hu",
                Password = "NagyTesztElek32",
                DriversLicenseNumber = "AA123456",
                DriversLicenseDate = new DateOnly(),
                AddressStreetHouse = "Zrínyi Ilona utca 12.",
                AddressSettlement = "Szombathely",
                AddressZipcode = "9700"
            })) as OkObjectResult;
            
            Assert.IsNotNull(putResult);
            #endregion
            
            #region GetOwner
            _controller.SetAuthUser(1, UserRole.User);
            
            var result = (await _controller!.GetUserById(1)) as OkObjectResult;
            Assert.IsNotNull(result);
            var ownerUser = JsonConvert.DeserializeObject<User>((string)result.Value);
            Assert.IsNotNull(ownerUser);
            
            Assert.AreEqual(ownerUser.Id, 1);
            Assert.AreEqual("36201234567", ownerUser.Phone); // In Relation
            Assert.AreEqual("123456AA", ownerUser.IdCardNumber); // Owner Only
            Assert.IsNull(ownerUser.Password); // Admin Only
            #endregion
            
            Assert.AreEqual("Teszt Elek Péter", ownerUser.Name);
        }
    }
}
