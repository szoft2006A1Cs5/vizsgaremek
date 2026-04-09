namespace backend.Services.EmailService;

public interface IEmailService
{
    public Task SendPasswordResetEmailAsync(string address, string token);
    public Task SendConfirmEmailAsync(string address, string token);
}