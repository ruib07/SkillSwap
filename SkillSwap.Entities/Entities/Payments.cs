using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSwap.Entities.Entities;

public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2
}

public class Payments
{
    public Guid Id { get; set; }
    public Guid PayerId { get; set; }
    public Users Payer { get; set; }
    public Guid MentorId { get; set; }
    public Users Mentor { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime UpdatedAt { get; set; }
}
