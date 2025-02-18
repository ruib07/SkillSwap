using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Constants;
using SkillSwap.Services.Services;
using static SkillSwap.Server.Models.Responses;

namespace SkillSwap.Server.Controllers;

[Route("skills")]
public class SkillsController : ControllerBase
{
    private readonly SkillsService _skills;

    public SkillsController(SkillsService skills)
    {
        _skills = skills;
    }

    // GET skills
    [HttpGet]
    public async Task<ActionResult<List<Skills>>> GetAllSkills()
    {
        var skills = await _skills.GetAllSkills();

        return Ok(skills);
    }

    // GET skills/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSkillById(Guid id)
    {
        var skill = await _skills.GetSkillById(id);

        return Ok(skill);
    }

    // TODO Get skills by user

    // POST skills
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPost]
    public async Task<ActionResult<Skills>> CreateSkill([FromBody] Skills skill)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _skills.CreateSkill(skill);

        return CreatedAtAction(nameof(GetSkillById), new { id = skill.Id }, new CreationResponse()
        {
            Message = "Skill created successfully.",
            Id = skill.Id
        });
    }

    // PUT skills/{skillId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPut("{skillId}")]
    public async Task<IActionResult> UpdateSkill(Guid skillId, [FromBody] Skills updateSkill)
    {
        await _skills.UpdateSkill(skillId, updateSkill);
        return Ok("Skill updated successfully.");
    }

    // DELETE skills/{skillId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpDelete("{skillId}")]
    public async Task<IActionResult> DeleteSkill(Guid skillId)
    {
        await _skills.DeleteSkill(skillId);

        return NoContent();
    }
}
