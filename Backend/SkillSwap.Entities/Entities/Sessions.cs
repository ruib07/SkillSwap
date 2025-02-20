using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSwap.Entities.Entities;

public class Sessions
{
    public Guid Id { get; set; }
    public Guid MentorshipRequestId { get; set; }
    public MentorshipRequests MentorshipRequest { get; set; }
    public DateTime SessionTime { get; set; }
    public int Duration { get; set; }
    public string VideoLink { get; set; }
    public decimal Amount { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime UpdatedAt { get; set; }
}
