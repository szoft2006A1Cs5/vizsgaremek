namespace backend.UnitTests.Tests;

[TestClass]
public class NotificationControllerTests
{
    [TestMethod]
    public async Task GetNotifications_Ok() {}
    
    [TestMethod]
    public async Task GetNotifications_Unauthorized() {}
    
    [TestMethod]
    public async Task GetNotificationById_Ok() {}
    
    [TestMethod]
    public async Task GetNotificationById_Unauthorized() {}
    
    [TestMethod]
    public async Task GetNotificationById_NotFound() {}
    
    [TestMethod]
    public async Task PostNotification_Ok() {}
    
    [TestMethod]
    public async Task PostNotification_Unauthorized() {}
    
    [TestMethod]
    public async Task PostNotification_NotFound() {}
    
    [TestMethod]
    public async Task DeleteNotification_Ok() {}
    
    [TestMethod]
    public async Task DeleteNotification_Unauthorized() {}
    
    [TestMethod]
    public async Task DeleteNotification_NotFound() {}
}
