using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Constants;
using SkillSwap.Server.Models.DTOs;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Server.Controllers;

[Route("userskills")]
[ApiController]
public class UserSkillsController : ControllerBase
{
    private readonly IUserSkills _userSkills;

    public UserSkillsController(IUserSkills userSkills)
    {
        _userSkills = userSkills;
    }

    // GET userskills/byuser/{userId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("byuser/{userId}")]
    public async Task<ActionResult<List<Skills>>> GetUserSkillsByUser(Guid userId)
    {
        var userSkills = await _userSkills.GetUserSkillsByUser(userId);

        return Ok(userSkills);
    }

    // POST userskills
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPost]
    public async Task<IActionResult> CreateUserSkill([FromBody] UserSkillDto userSkillDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _userSkills.CreateUserSkill(userSkillDto.UserId, userSkillDto.SkillId);

        return StatusCode(StatusCodes.Status201Created, "User skill created successfully.");
    }

    // DELETE userskills/{userId}/{skillId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpDelete("{userId}/{skillId}")]
    public async Task<IActionResult> DeleteUserSkill(Guid userId, Guid skillId)
    {
        await _userSkills.DeleteUserSkill(userId, skillId);

        return NoContent();
    }
}
