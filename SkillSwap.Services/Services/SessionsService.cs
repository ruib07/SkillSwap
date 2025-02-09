using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Services.Services;

public class SessionsService : ISessions
{
    private readonly SkillSwapDbContext _context;
    private DbSet<Sessions> Sessions => _context.Sessions;

    public SessionsService(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<Sessions> GetSessionById(Guid id)
    {
        var session = await Sessions.FirstOrDefaultAsync(s => s.Id == id);

        if (session == null) ErrorHelper.ThrowNotFoundException("Session not found.");

        return session;
    }

    public async Task<List<Sessions>> GetSessionsByMentorshipRequestId(Guid mentorshipRequestId)
    {
        var sessionsByMentorshipRequest = await Sessions.AsNoTracking()
                                          .Where(smr => smr.MentorshipRequestId == mentorshipRequestId)
                                          .ToListAsync();

        if (sessionsByMentorshipRequest == null || sessionsByMentorshipRequest.Count == 0)
            ErrorHelper.ThrowNotFoundException("No sessions found to that mentorship request.");

        return sessionsByMentorshipRequest;
    }

    public async Task<Sessions> CreateSession(Sessions session)
    {
        await Sessions.AddAsync(session);
        await _context.SaveChangesAsync();

        return session;
    }

    public async Task<Sessions> UpdateSession(Guid id, Sessions updateSession)
    {
        var currentSession = await GetSessionById(id);

        currentSession.SessionTime = updateSession.SessionTime;
        currentSession.Duration = updateSession.Duration;
        currentSession.VideoLink = updateSession.VideoLink;

        await _context.SaveChangesAsync();

        return currentSession;
    }

    public async Task DeleteSession(Guid id)
    {
        var deleteSession = await GetSessionById(id);

        Sessions.Remove(deleteSession);
        await _context.SaveChangesAsync();
    }
}
