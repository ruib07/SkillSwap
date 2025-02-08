using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Constants;
using SkillSwap.Server.Exceptions;
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

        if (user == null) return NotFound();

        return Ok(user);
    }

    // POST users
    [HttpPost]
    public async Task<ActionResult<Users>> CreateUser([FromBody] Users user)
    {
        await _users.CreateUser(user);

        return StatusCode(StatusCodes.Status201Created, "User created successffully.");
    }

    // POST users/recoverpassword
    [HttpPost("recoverpassword")]
    public async Task<IActionResult> UserRecoverPassword([FromBody] RecoverPasswordRequest request)
    {
        try
        {
            var token = await _users.GeneratePasswordResetToken(request.Email);
            await _emailService.SendPasswordResetEmail(request.Email, token);

            return Ok();
        }
        catch (Exception)
        {
            return BadRequest("Something went wrong.");
        }
    }

    // PUT users/updatepassword
    [HttpPut("updatepassword")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        try
        {
            await _users.UpdatePassword(request.Token, request.NewPassword, request.ConfirmNewPassword);

            return Ok();
        }
        catch (Exception)
        {
            return BadRequest("Something went wrong.");
        }
    }

    // PUT users/{userId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] Users updateUser)
    {
        try
        {
            var updatedUser = await _users.UpdateUser(userId, updateUser);
            return Ok(updatedUser);
        }
        catch (NotFoundException ex) 
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ConflictException ex) 
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception) 
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred." });
        }
    }


    // PATCH users/{userId}/balance
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
