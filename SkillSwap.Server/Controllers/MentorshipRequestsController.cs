using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Constants;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Server.Controllers;

[Route("mentorshiprequests")]
[ApiController]
public class MentorshipRequestsController : ControllerBase
{
    private readonly IMentorshipRequests _mentorshipRequests;

    public MentorshipRequestsController(IMentorshipRequests mentorshipRequests)
    {
        _mentorshipRequests = mentorshipRequests;
    }

    // GET mentorshiprequests/{mentorshiprequestId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("{mentorshiprequestId}")]
    public async Task<IActionResult> GetMentorshipRequestById(Guid mentorshiprequestId)
    {
        var mentorshipReq = await _mentorshipRequests.GetMentorshipRequestById(mentorshiprequestId);

        return Ok(mentorshipReq);
    }

    // GET mentorshiprequests/bylearner/{learnerId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("bylearner/{learnerId}")]
    public async Task<ActionResult<List<MentorshipRequests>>> GetMentorshipRequestsbyLearnerId(Guid learnerId)
    {
        var mentorshipReqsByLearner = await _mentorshipRequests.GetMentorshipRequestsbyLearnerId(learnerId);

        return Ok(mentorshipReqsByLearner);
    }

    // GET mentorshiprequests/bymentor/{mentorId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("bymentor/{mentorId}")]
    public async Task<ActionResult<List<MentorshipRequests>>> GetMentorshipRequestsbyMentorId(Guid mentorId)
    {
        var mentorshipReqsByMentor = await _mentorshipRequests.GetMentorshipRequestsbyMentorId(mentorId);

        return Ok(mentorshipReqsByMentor);
    }

    // POST mentorshiprequests
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPost]
    public async Task<ActionResult<MentorshipRequests>> CreateMentorshipRequest([FromBody] MentorshipRequests mentorshipRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _mentorshipRequests.CreateMentorshipRequest(mentorshipRequest);

        return StatusCode(StatusCodes.Status201Created, "Mentorship request created successfully.");
    }

    // PUT mentorshiprequests/{mentorshiprequestId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPut("{mentorshiprequestId}")]
    public async Task<IActionResult> UpdateSkill(Guid mentorshiprequestId, [FromBody] MentorshipRequests updateMentorshipRequest)
    {
        var updatedMentorshipReq = await _mentorshipRequests.UpdateMentorshipRequest(mentorshiprequestId, updateMentorshipRequest);

        return Ok("Mentorship request updated successfully.");
    }

    // DELETE mentorshiprequests/{mentorshiprequestId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpDelete("{mentorshiprequestId}")]
    public async Task<IActionResult> DeleteSkill(Guid mentorshiprequestId)
    {
        await _mentorshipRequests.DeleteMentorshipRequest(mentorshiprequestId);

        return NoContent();
    }
}
