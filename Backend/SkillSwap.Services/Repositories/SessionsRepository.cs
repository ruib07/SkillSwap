using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Repositories.Interfaces;

namespace SkillSwap.Services.Repositories;

public class SessionsRepository : ISessionsRepository
{
    private readonly SkillSwapDbContext _context;
    private DbSet<Sessions> Sessions => _context.Sessions;

    public SessionsRepository(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<Sessions> GetSessionById(Guid id)
    {
        return await Sessions.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Sessions>> GetSessionsByMentorshipRequestId(Guid mentorshipRequestId)
    {
        return await Sessions.AsNoTracking().Where(smr => smr.MentorshipRequestId == mentorshipRequestId).ToListAsync();
    }

    public async Task<Sessions> CreateSession(Sessions session)
    {
        await Sessions.AddAsync(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task UpdateSession(Sessions session)
    {
        _context.Sessions.Update(session);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSession(Guid id)
    {
        var deleteSession = await GetSessionById(id);

        Sessions.Remove(deleteSession);
        await _context.SaveChangesAsync();
    }
}
