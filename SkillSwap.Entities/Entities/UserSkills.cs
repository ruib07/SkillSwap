using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSwap.Entities.Entities;

public class UserSkills
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Users User { get; set; }
    public Guid SkillId { get; set; }
    public Skills Skill { get; set; }
    public string Type { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }
}
