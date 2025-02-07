namespace SkillSwap.Entities.Entities;

public class PasswordResetToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Users User { get; set; }
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
}
