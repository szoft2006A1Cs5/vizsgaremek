namespace backend.Services.EmailService;

public interface IEmailService
{
    /// <summary>
    /// Elkuld egy jelszo visszaallito kodot a megadott email cimre.
    /// </summary>
    /// <param name="address">A cimzett emailje.</param>
    /// <param name="token">Az elkuldendo kod</param>
    public Task SendPasswordResetEmailAsync(string address, string token);
    public Task SendConfirmEmailAsync(string address, string token);
}