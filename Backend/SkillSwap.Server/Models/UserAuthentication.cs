namespace SkillSwap.Server.Models;

public class UserAuthentication
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public LoginResponse()
        {
            TokenType = "Bearer";
        }

        public LoginResponse(string accessToken) : this()
        {
            AccessToken = accessToken;
        }

        public string AccessToken { get; set; }
        public string TokenType { get; set; }
    }
}
