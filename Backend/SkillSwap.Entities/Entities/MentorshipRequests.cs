using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSwap.Entities.Entities;

public enum MentorshipRequestsStatus
{
    Pending = 0,
    Accepted = 1,
    Completed = 2,
    Cancelled = 3
}

public class MentorshipRequests
{
    public Guid Id { get; set; }
    public Guid MentorId { get; set; }
    public Users Mentor { get; set; }
    public Guid LearnerId { get; set; }
    public Users Learner { get; set; }
    public Guid SkillId { get; set; }
    public Skills Skill { get; set; }
    public MentorshipRequestsStatus Status { get; set; }
    public DateTime ScheduledAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }
}
