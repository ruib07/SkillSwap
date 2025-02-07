using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Services.Services;

public class SessionsService : ISessions
{
    private readonly SkillSwapDbContext _context;

    public SessionsService(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<Sessions> GetSessionById(Guid id)
    {
        var session = await _context.Sessions.AsNoTracking()
                      .FirstOrDefaultAsync(s => s.Id == id);

        if (session == null) ErrorHelper.ThrowNotFoundException("Session not found.");

        return session;
    }

    public async Task<List<Sessions>> GetSessionsByMentorshipRequestId(Guid mentorshipRequestId)
    {
        var sessionsByMentorshipRequest = await _context.Sessions.AsNoTracking()
                                          .Where(smr => smr.MentorshipRequestId == mentorshipRequestId)
                                          .ToListAsync();

        if (sessionsByMentorshipRequest == null || sessionsByMentorshipRequest.Count == 0)
            ErrorHelper.ThrowNotFoundException("No sessions found to that mentorship request.");

        return sessionsByMentorshipRequest;
    }

    public async Task<Sessions> CreateSession(Sessions session)
    {
        await _context.Sessions.AddAsync(session);
        await _context.SaveChangesAsync();

        return session;
    }

    public async Task<Sessions> UpdateSession(Guid id, Sessions updateSession)
    {
        var currentSession = await _context.Sessions.FirstOrDefaultAsync(cs => cs.Id == id);

        if (currentSession == null) ErrorHelper.ThrowNotFoundException("Session not found.");

        currentSession.SessionTime = updateSession.SessionTime;
        currentSession.Duration = updateSession.Duration;
        currentSession.VideoLink = updateSession.VideoLink;

        await _context.SaveChangesAsync();

        return currentSession;
    }

    public async Task DeleteSession(Guid id)
    {
        var deleteSession = await _context.Sessions.FirstOrDefaultAsync(ds => ds.Id == id);

        if (deleteSession == null) ErrorHelper.ThrowNotFoundException("Session not found.");

        _context.Sessions.Remove(deleteSession);
        await _context.SaveChangesAsync();
    }
}
