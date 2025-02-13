using Microsoft.Extensions.Configuration;
using Moq;
using SkillSwap.Services.Services.Email;
using System.Net.Mail;

namespace SkillSwap.Tests.Services;

[TestFixture]
public class EmailPasswordResetsServiceTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<IEmailSender> _mockEmailSender;
    private EmailPasswordResetsService _emailPasswordResetsService;

    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.SetupGet(c => c["EmailSettings:Username"]).Returns("test@example.com");
        _mockConfiguration.SetupGet(c => c["EmailSettings:Password"]).Returns("password");
        _mockConfiguration.SetupGet(c => c["EmailSettings:Host"]).Returns("smtp.example.com");
        _mockConfiguration.SetupGet(c => c["EmailSettings:Port"]).Returns("587");
        _mockConfiguration.SetupGet(c => c["EmailSettings:EnableSsl"]).Returns("true");

        _mockEmailSender = new Mock<IEmailSender>();
        _emailPasswordResetsService = new EmailPasswordResetsService(_mockEmailSender.Object, _mockConfiguration.Object);
    }

    [Test]
    public async Task SendPasswordResetEmail_ShouldSendEmail()
    {
        var email = "usertest@email.com";
        var token = "test-token";

        await _emailPasswordResetsService.SendPasswordResetEmail(email, token);

        _mockEmailSender.Verify(x => x.SendEmailAsync(It.Is<MailMessage>(msg =>
            msg.To[0].Address == email &&
            msg.Subject == "Password Recovery" &&
            msg.Body.Contains(token)
        )), Times.Once);
    }
}
