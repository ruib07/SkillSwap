using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Constants;
using SkillSwap.Services.Interfaces;
using static SkillSwap.Server.Models.Responses;

namespace SkillSwap.Server.Controllers;

[Route("mentorship-requests")]
[ApiController]
public class MentorshipRequestsController : ControllerBase
{
    private readonly IMentorshipRequests _mentorshipRequests;

    public MentorshipRequestsController(IMentorshipRequests mentorshipRequests)
    {
        _mentorshipRequests = mentorshipRequests;
    }

    // GET mentorship-requests/{id}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMentorshipRequestById(Guid id)
    {
        var mentorshipReq = await _mentorshipRequests.GetMentorshipRequestById(id);

        return Ok(mentorshipReq);
    }

    // GET mentorship-requests/bylearner/{learnerId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("bylearner/{learnerId}")]
    public async Task<ActionResult<List<MentorshipRequests>>> GetMentorshipRequestsbyLearnerId(Guid learnerId)
    {
        var mentorshipReqsByLearner = await _mentorshipRequests.GetMentorshipRequestsbyLearnerId(learnerId);

        return Ok(mentorshipReqsByLearner);
    }

    // GET mentorship-requests/bymentor/{mentorId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("bymentor/{mentorId}")]
    public async Task<ActionResult<List<MentorshipRequests>>> GetMentorshipRequestsbyMentorId(Guid mentorId)
    {
        var mentorshipReqsByMentor = await _mentorshipRequests.GetMentorshipRequestsbyMentorId(mentorId);

        return Ok(mentorshipReqsByMentor);
    }

    // POST mentorship-requests
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPost]
    public async Task<ActionResult<MentorshipRequests>> CreateMentorshipRequest([FromBody] MentorshipRequests mentorshipRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _mentorshipRequests.CreateMentorshipRequest(mentorshipRequest);

        return CreatedAtAction(nameof(GetMentorshipRequestById), new { id = mentorshipRequest.Id }, new CreationResponse()
        {
            Message = "Mentorship request created successfully.",
            Id = mentorshipRequest.Id
        });
    }

    // PUT mentorship-requests/{mentorshiprequestId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPut("{mentorshiprequestId}")]
    public async Task<IActionResult> UpdateSkill(Guid mentorshiprequestId, [FromBody] MentorshipRequests updateMentorshipRequest)
    {
        var updatedMentorshipReq = await _mentorshipRequests.UpdateMentorshipRequest(mentorshiprequestId, updateMentorshipRequest);

        return Ok("Mentorship request updated successfully.");
    }

    // DELETE mentorship-requests/{mentorshiprequestId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpDelete("{mentorshiprequestId}")]
    public async Task<IActionResult> DeleteSkill(Guid mentorshiprequestId)
    {
        await _mentorshipRequests.DeleteMentorshipRequest(mentorshiprequestId);

        return NoContent();
    }
}
