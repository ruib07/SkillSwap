using System.ComponentModel.DataAnnotations;

namespace SkillSwap.Server.Models;

public class UpdateBalance
{
    public class UpdateBalanceRequest
    {
        [Required]
        public decimal? Balance { get; set; }
    }

    public class UpdateBalanceResponse
    {
        public string Message { get; set; }
        public decimal UpdatedBalance { get; set; }
    }

    public class UpdateBalanceBadRequest
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}
