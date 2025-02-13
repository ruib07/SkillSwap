using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace SkillSwap.Services.Services.Email;

public interface IEmailSender
{
    Task SendEmailAsync(MailMessage message);
}

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public SmtpEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(MailMessage message)
    {
        var smtp = new SmtpClient()
        {
            Host = _configuration["EmailSettings:Host"],
            Port = int.Parse(_configuration["EmailSettings:Port"]),
            EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"])
        };

        await smtp.SendMailAsync(message);
    }
}
