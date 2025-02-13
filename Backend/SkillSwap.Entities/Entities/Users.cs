using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace SkillSwap.Entities.Entities;

public class Users
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    private string _password;
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{9,}$",
        ErrorMessage = "{0} must contain at least 9 characters, an uppercase letter," +
            " a lowercase letter, a number and a special character.")]
    public string Password
    {
        get => _password;
        set
        {
            if (value != null)
            {
                if (Regex.IsMatch(value, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{9,}$"))
                {
                    _password = value;
                }
            }
            else
            {
                throw new ArgumentException("Password doest meet complexity requirements.");
            }
        }
    }

    public string Bio { get; set; }
    public string ProfilePicture { get; set; }
    public decimal Balance { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime UpdatedAt { get; set; }

    public ICollection<Skills> Skills { get; set; } = new List<Skills>();
}
