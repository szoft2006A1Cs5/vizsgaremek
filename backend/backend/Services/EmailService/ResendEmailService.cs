using backend.Common;
using Resend;

namespace backend.Services.EmailService;

public class ResendEmailService : IEmailService
{
    private readonly IResend _resend;

    public ResendEmailService(IResend resend)
    {
        _resend = resend;
    }
    
    public async Task SendPasswordResetEmailAsync(string address, string token)
    {
        var message = new EmailMessage();
        
        message.From = "CoMove <noreply@comove.app>";
        message.To.Add(address);
        message.Subject = "CoMove - Jelszó visszaállítása";
        message.HtmlBody = "<h1>Állítsd vissza a jelszavadat az alábbi kóddal:</h1>" +
                           $"<h2>{token}</h2>" +
                           $"<p>A kód csak a következő {Config.TokenValidMins} percben érvényes!</p>";

        await _resend.EmailSendAsync(message);
    }

    public async Task SendConfirmEmailAsync(string address, string token)
    {
        
    }
}