using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Constants;
using SkillSwap.Services.Services;
using static SkillSwap.Server.Models.Responses;

namespace SkillSwap.Server.Controllers;

[Route("sessions")]
public class SessionsController : ControllerBase
{
    private readonly SessionsService _sessions;

    public SessionsController(SessionsService sessions)
    {
        _sessions = sessions;
    }

    // GET sessions/{id}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSessionById(Guid id)
    {
        var session = await _sessions.GetSessionById(id);

        return Ok(session);
    }

    // GET sessions/bymentorshiprequest/{mentorshipRequestId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("bymentorshiprequest/{mentorshipRequestId}")]
    public async Task<ActionResult<List<Sessions>>> GetSessionsByMentorshipRequestId(Guid mentorshipRequestId)
    {
        var sessionsByMentorshipReq = await _sessions.GetSessionsByMentorshipRequestId(mentorshipRequestId);

        return Ok(sessionsByMentorshipReq);
    }

    // POST sessions
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPost]
    public async Task<ActionResult<Sessions>> CreateSession([FromBody] Sessions session)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _sessions.CreateSession(session);

        return CreatedAtAction(nameof(GetSessionById), new { id = session.Id }, new CreationResponse()
        {
            Message = "Session created successfully.",
            Id = session.Id
        });
    }

    // PUT sessions/{sessionId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPut("{sessionId}")]
    public async Task<IActionResult> UpdateSession(Guid sessionId, [FromBody] Sessions updateSession)
    {
        await _sessions.UpdateSession(sessionId, updateSession);
        return Ok("Session updated successfully.");
    }

    // DELETE sessions/{sessionId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpDelete("{sessionId}")]
    public async Task<IActionResult> DeleteSession(Guid sessionId)
    {
        await _sessions.DeleteSession(sessionId);
        return NoContent();
    }
}
