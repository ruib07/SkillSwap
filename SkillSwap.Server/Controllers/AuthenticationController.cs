using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Server.Constants;
using SkillSwap.Server.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static SkillSwap.Server.Models.UserAuthentication;

namespace SkillSwap.Server.Controllers;

[Route("auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly SkillSwapDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthenticationController(SkillSwapDbContext context, JwtSettings jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody, Required] LoginRequest loginRequest)
    {
        if (loginRequest == null) return BadRequest("Email and password are mandatory.");
        if (string.IsNullOrWhiteSpace(loginRequest.Email)) return BadRequest("Email is required.");
        if (string.IsNullOrWhiteSpace(loginRequest.Password)) return BadRequest("Password is required.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);

        if (user == null) return Unauthorized("User not found.");
        if (!VerifyPassword(loginRequest.Password, user.Password)) return Unauthorized("Incorrect password!");

        var claims = new List<Claim>()
        {
            new("Id", user.Id.ToString()),
            new(ClaimTypes.Role, ApiConstants.PolicyUser),
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new("Bio", user.Bio ?? ""),
            new("ProfilePicture", user.ProfilePicture ?? ""),
            new("Balance", user.Balance.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Key));
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return Ok(new LoginResponse(jwtToken));
    }

    #region Private Methods

    private static bool VerifyPassword(string providedPassword, string storedHash)
    {
        byte[] hashBytes = Convert.FromBase64String(storedHash);
        byte[] salt = hashBytes[..16];

        using var pbkdf2 = new Rfc2898DeriveBytes(providedPassword, salt, 10000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32);

        return hash.SequenceEqual(hashBytes[16..]);
    }

    #endregion
}
