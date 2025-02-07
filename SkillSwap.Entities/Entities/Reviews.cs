using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSwap.Entities.Entities;

public class Reviews
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public Sessions Session { get; set; }
    public Guid ReviewerId { get; set; }
    public Users Reviewer { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int Rating { get; set; }
    public string Comments { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }
}
