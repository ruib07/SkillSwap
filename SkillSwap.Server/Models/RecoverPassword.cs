using System.ComponentModel.DataAnnotations;

namespace SkillSwap.Server.Models;

public class RecoverPassword
{
    public class RecoverPasswordRequest
    {
        [Required]
        public string Email { get; set; }
    }

    public class UpdatePasswordRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmNewPassword { get; set; }
    }
}
