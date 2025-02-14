using Microsoft.Extensions.Configuration;
using System.Net.Mail;

namespace SkillSwap.Services.Services.Email;

public interface IEmailPasswordResets
{
    Task SendPasswordResetEmail(string email, string token);
}

public class EmailPasswordResetsService : IEmailPasswordResets
{
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public EmailPasswordResetsService(IEmailSender emailSender, IConfiguration configuration)
    {
        _emailSender = emailSender;
        _configuration = configuration;
    }

    public async Task SendPasswordResetEmail(string email, string token)
    {
        var resetLink = $"http://localhost:3000/RecoverPassword/ChangePassword?token={token}";
        var fromAddress = new MailAddress(_configuration["EmailSettings:Username"], "SkillSwap Support");
        var toAddress = new MailAddress(email);

        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = "Password Recovery",
            Body = $"Click on the link to reset your password: {resetLink}"
        };

        await _emailSender.SendEmailAsync(message);
    }
}
