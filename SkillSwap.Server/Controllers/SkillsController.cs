﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Constants;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Server.Controllers;

[Route("skills")]
[ApiController]
public class SkillsController : ControllerBase
{
    private ISkills _skills;

    public SkillsController(ISkills skills)
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

    // GET skills/{skillId}
    [HttpGet("{skillId}")]
    public async Task<IActionResult> GetSkillById(Guid skillId)
    {
        var skill = await _skills.GetSkillById(skillId);

        return Ok(skill);
    }

    // POST skills
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPost]
    public async Task<ActionResult<Skills>> CreateSkill([FromBody] Skills skill)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _skills.CreateSkill(skill);

        return StatusCode(StatusCodes.Status201Created, "Skill created successffully.");
    }

    // PUT skills/{skillId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPut("{skillId}")]
    public async Task<IActionResult> UpdateSkill(Guid skillId, [FromBody] Skills updateSkill)
    {
        var updatedSkill = await _skills.UpdateSkill(skillId, updateSkill);

        return Ok(updatedSkill);
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
