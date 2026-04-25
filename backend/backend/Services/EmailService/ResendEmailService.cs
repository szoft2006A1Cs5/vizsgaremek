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
        
        message.From = "CoMove <comove@comove.app>";
        message.To.Add(address);
        message.Subject = "CoMove - Jelszó visszaállítása";
        message.HtmlBody = "<h2>Állítsd vissza a jelszavadat az alábbi kóddal:</h2>" +
                           $"<h1>{token}</h1>" +
                           $"<h3>Az elfelejtett jelszó oldalon kattintson a 'Már van kódom' szövegre " +
                           $"a kód felhasználásához!</h3>" +
                           $"<p>A kód csak a következő {Config.TokenValidMins} percben érvényes!</p>";

        await _resend.EmailSendAsync(message);
    }

    public async Task SendConfirmEmailAsync(string address, string token)
    {
        var message = new EmailMessage();
        
        message.From = "CoMove <comove@comove.app>";
        message.To.Add(address);
        message.Subject = "CoMove - E-Mail megerősítése";
        message.HtmlBody = "<h2>Erősítsd meg az e-mailed az alábbi kóddal:</h2>" +
                           $"<h1>{token}</h1>" +
                           $"<p>A kód csak a következő {Config.TokenValidMins} percben érvényes!</p>";

        await _resend.EmailSendAsync(message);
    }
}