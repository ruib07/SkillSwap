using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Constants;
using SkillSwap.Services.Services;
using SkillSwap.Services.Services.Email;
using System.Net;
using static SkillSwap.Server.Models.RecoverPassword;
using static SkillSwap.Server.Models.Responses;
using static SkillSwap.Server.Models.UpdateBalance;

namespace SkillSwap.Server.Controllers;

[Route("users")]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;
    private readonly IEmailPasswordResets _emailService;

    public UsersController(UsersService usersService, IEmailPasswordResets emailService)
    {
        _usersService = usersService;
        _emailService = emailService;
    }

    // GET users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _usersService.GetUserById(id);
        return Ok(user);
    }

    // POST users
    [HttpPost]
    public async Task<ActionResult<Users>> CreateUser([FromBody] Users user)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var createdUser = await _usersService.CreateUser(user);

        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, new CreationResponse
        {
            Message = "User created successfully.",
            Id = createdUser.Id
        });
    }

    // POST users/recoverpassword
    [HttpPost("recoverpassword")]
    public async Task<IActionResult> RecoverPasswordSendEmail([FromBody] RecoverPasswordRequest request)
    {
        var token = await _usersService.GeneratePasswordResetToken(request.Email);
        await _emailService.SendPasswordResetEmail(request.Email, token);

        return Ok();
    }

    // PUT users/updatepassword
    [HttpPut("updatepassword")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        await _usersService.UpdatePassword(request.Token, request.NewPassword, request.ConfirmNewPassword);
        return Ok();
    }

    // PUT users/{userId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] Users updateUser)
    {
        await _usersService.UpdateUser(userId, updateUser);
        return Ok("User updated successfully.");
    }

    // PATCH users/{userId}/balance
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPatch("{userId}/balance")]
    public async Task<IActionResult> UpdateBalance(Guid userId, [FromBody] UpdateBalanceRequest request)
    {
        if (!request.Balance.HasValue) return BadRequest(new UpdateBalanceBadRequest
        { 
            Message = "Balance is required.", 
            StatusCode = (int)HttpStatusCode.BadRequest
        });

        var updatedBalance = await _usersService.UpdateBalance(userId, request.Balance.Value);

        return Ok(new UpdateBalanceResponse
        { 
            Message = "Balance updated successfully.", 
            UpdatedBalance = updatedBalance 
        });
    }

    // DELETE users/{userId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        await _usersService.DeleteUser(userId);
        return NoContent();
    }
}
