namespace backend.UnitTests.Tests;

[TestClass]
public class AuthControllerTests
{
    [TestInitialize]
    public void Initialize()
    {

    }

    [TestCleanup]
    public void Cleanup()
    {

    }

    [TestMethod]
    public async Task PostLogin_Ok() {}
    
    [TestMethod]
    public async Task PostLogin_Unauthorized() {}
    
    [TestMethod]
    public async Task PostRegister_Ok() {}
    
    [TestMethod]
    public async Task PostRegister_BadRequest() {}
    
    [TestMethod]
    public async Task PostRegister_Conflict() {}
}
