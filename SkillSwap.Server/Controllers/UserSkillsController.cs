using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Constants;
using SkillSwap.Server.Models.DTOs;
using SkillSwap.Services.Services;

namespace SkillSwap.Server.Controllers;

[Route("user-skills")]
[ApiController]
public class UserSkillsController : ControllerBase
{
    private readonly UserSkillsService _userSkills;

    public UserSkillsController(UserSkillsService userSkills)
    {
        _userSkills = userSkills;
    }

    // GET user-skills/byuser/{userId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("byuser/{userId}")]
    public async Task<ActionResult<List<Skills>>> GetUserSkillsByUser(Guid userId)
    {
        var userSkills = await _userSkills.GetUserSkillsByUser(userId);

        return Ok(userSkills);
    }

    // POST user-skills
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPost]
    public async Task<IActionResult> CreateUserSkill([FromBody] UserSkillDto userSkillDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _userSkills.CreateUserSkill(userSkillDto.UserId, userSkillDto.SkillId);

        return StatusCode(StatusCodes.Status201Created, "User skill created successfully.");
    }

    // DELETE user-skills/{userId}/{skillId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpDelete("{userId}/{skillId}")]
    public async Task<IActionResult> DeleteUserSkill(Guid userId, Guid skillId)
    {
        await _userSkills.DeleteUserSkill(userId, skillId);

        return NoContent();
    }
}
