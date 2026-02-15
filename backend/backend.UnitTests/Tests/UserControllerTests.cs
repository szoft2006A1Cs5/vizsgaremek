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
        TestingEnvironment _environment;
        UserController? _controller;

        [TestInitialize]
        public void Initialize()
        {
            _environment = MockContext.CreateContext();

            _controller = new UserController(_environment.Context, _environment.AuthManager);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = MockContext.GetClaimsPrincipalFor(1, UserRole.User)
                }
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            _environment.Context.Dispose();
        }
        
        [TestMethod]
        public async Task GetUserPublic_Ok()
        {
            var result = (await _controller!.Get(1)) as OkObjectResult;
            Assert.IsNotNull(result);
            var user = JsonConvert.DeserializeObject<User>((string)result.Value);
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public async Task GetUser_NotFound()
        {
            var result = (await _controller!.Get(2)) as NotFoundResult;
            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public async Task GetUserOwner_Ok() {}
        
        [TestMethod]
        public async Task GetUserInRelation_Ok() {}
        
        [TestMethod]
        public async Task GetUserAdmin_Ok() {}
        
        [TestMethod]
        public async Task PutUser_Ok() {}
        
        [TestMethod]
        public async Task PutUser_NotFound() {}
        
        [TestMethod]
        public async Task PutUser_Forbidden() {}
        
        [TestMethod]
        public async Task PutUser_Conflict() {}
        
        [TestMethod]
        public async Task DeleteUser_Ok() {}
        
        [TestMethod]
        public async Task DeleteUser_NotFound() {}
        
        [TestMethod]
        public async Task DeleteUser_Forbidden() {}
    }
}
