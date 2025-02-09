using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Constants;
using SkillSwap.Server.Models;
using SkillSwap.Services.Interfaces;
using static SkillSwap.Server.Models.RecoverPassword;

namespace SkillSwap.Server.Controllers;

[Route("users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUsers _users;
    private readonly IEmailPasswordResets _emailService;

    public UsersController(IUsers users, IEmailPasswordResets emailService)
    {
        _users = users;
        _emailService = emailService;
    }

    // GET users/{userId}
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        var user = await _users.GetUserById(userId);

        return Ok(user);
    }

    // POST users
    [HttpPost]
    public async Task<ActionResult<Users>> CreateUser([FromBody] Users user)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _users.CreateUser(user);

        return StatusCode(StatusCodes.Status201Created, "User created successfully.");
    }

    // POST users/recoverpassword
    [HttpPost("recoverpassword")]
    public async Task<IActionResult> RecoverPasswordSendEmail([FromBody] RecoverPasswordRequest request)
    {
        var token = await _users.GeneratePasswordResetToken(request.Email);
        await _emailService.SendPasswordResetEmail(request.Email, token);

        return Ok();
    }

    // PUT users/updatepassword
    [HttpPut("updatepassword")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        await _users.UpdatePassword(request.Token, request.NewPassword, request.ConfirmNewPassword);

        return Ok();
    }

    // PUT users/{userId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] Users updateUser)
    {
        var updatedUser = await _users.UpdateUser(userId, updateUser);

        return Ok("User updated successfully.");
    }


    // PATCH users/{userId}/balance
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPatch("{userId}/balance")]
    public async Task<IActionResult> UpdateBalance(Guid userId, [FromBody] UpdateBalanceRequest request)
    {
        if (!request.Balance.HasValue) return BadRequest(new { message = "Balance is required." });

        var updatedBalance = await _users.UpdateBalance(userId, request.Balance.Value);

        return Ok(updatedBalance);
    }

    // DELETE users/{userId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        await _users.DeleteUser(userId);

        return NoContent();
    }
}
