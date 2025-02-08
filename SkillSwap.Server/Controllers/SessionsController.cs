using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Constants;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Server.Controllers;

[Route("sessions")]
[ApiController]
public class SessionsController : ControllerBase
{
    private ISessions _sessions;

    public SessionsController(ISessions sessions)
    {
        _sessions = sessions;
    }

    // GET sessions/{sessionId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("{sessionId}")]
    public async Task<IActionResult> GetSessionById(Guid sessionId)
    {
        var session = await _sessions.GetSessionById(sessionId);

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

        return StatusCode(StatusCodes.Status201Created, "Session created successffully.");
    }

    // PUT sessions/{sessionId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPut("{sessionId}")]
    public async Task<IActionResult> UpdateSession(Guid sessionId, [FromBody] Sessions updateSession)
    {
        var updatedSession = await _sessions.UpdateSession(sessionId, updateSession);

        return Ok(updatedSession);
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
