using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSwap.Entities.Entities;

public class Skills
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime UpdatedAt { get; set; }
}
