using System.ComponentModel.DataAnnotations;

namespace SkillSwap.Server.Models;

public class UpdateBalanceRequest
{
    [Required]
    public decimal? Balance { get; set; }
}
