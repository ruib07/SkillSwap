namespace SkillSwap.Services.Interfaces;

public interface IEmailPasswordResets
{
    Task SendPasswordResetEmail(string email, string token);
}
