using SkillSwap.Services.Interfaces;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace SkillSwap.Services.Services;

public class EmailPasswordResetsService : IEmailPasswordResets
{
    private readonly IConfiguration _configuration;

    public EmailPasswordResetsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetEmail(string email, string token)
    {
        var resetLink = $"http://localhost:3000/Authentication/RecoverPassword/ChangePassword?token={token}";

        var fromAddress = new MailAddress(_configuration["EmailSettings:Username"], "SkillSwap Support");
        var toAddress = new MailAddress(email);
        string fromPassword = _configuration["EmailSettings:Password"];

        var smtp = new SmtpClient()
        {
            Host = _configuration["EmailSettings:Host"],
            Port = int.Parse(_configuration["EmailSettings:Port"]),
            EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        };

        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = "Password Recovery",
            Body = $"Click on the link to reset your password: {resetLink}"
        };
        await smtp.SendMailAsync(message);
    }
}
