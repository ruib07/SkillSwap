using SkillSwap.Entities.Entities;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;

namespace SkillSwap.Services.Services;

public class SessionsService 
{
    private readonly ISessionsRepository _sessionsRepository;

    public SessionsService(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<Sessions> GetSessionById(Guid id)
    {
        var session = await _sessionsRepository.GetSessionById(id);

        if (session == null) ErrorHelper.ThrowNotFoundException("Session not found.");

        return session;
    }

    public async Task<List<Sessions>> GetSessionsByMentorshipRequestId(Guid mentorshipRequestId)
    {
        var sessionsByMentorshipRequest = await _sessionsRepository.GetSessionsByMentorshipRequestId(mentorshipRequestId);

        if (sessionsByMentorshipRequest == null || sessionsByMentorshipRequest.Count == 0)
            ErrorHelper.ThrowNotFoundException("No sessions found to that mentorship request.");

        return sessionsByMentorshipRequest;
    }

    public async Task<Sessions> CreateSession(Sessions session)
    {
        return await _sessionsRepository.CreateSession(session);
    }

    public async Task<Sessions> UpdateSession(Guid id, Sessions updateSession)
    {
        var currentSession = await GetSessionById(id);

        currentSession.SessionTime = updateSession.SessionTime;
        currentSession.Duration = updateSession.Duration;
        currentSession.VideoLink = updateSession.VideoLink;
        currentSession.Amount = updateSession.Amount;

        await _sessionsRepository.UpdateSession(currentSession);
        return currentSession;
    }

    public async Task DeleteSession(Guid id)
    {
        await _sessionsRepository.DeleteSession(id);
    }
}
