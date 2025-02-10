using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Repositories.Interfaces;

namespace SkillSwap.Services.Repositories;

public class MentorshipRequestsRepository : IMentorshipRequestsRepository
{
    private readonly SkillSwapDbContext _context;
    private DbSet<MentorshipRequests> MentorshipRequests => _context.MentorshipRequests;

    public MentorshipRequestsRepository(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<MentorshipRequests> GetMentorshipRequestById(Guid id)
    {
        return await MentorshipRequests.FirstOrDefaultAsync(mr => mr.Id == id);
    }

    public async Task<List<MentorshipRequests>> GetMentorshipRequestsbyLearnerId(Guid learnerId)
    {
        return await MentorshipRequests.Where(mr => mr.LearnerId == learnerId).ToListAsync();
    }

    public async Task<List<MentorshipRequests>> GetMentorshipRequestsbyMentorId(Guid mentorId)
    {
        return await MentorshipRequests.AsNoTracking().Where(mr => mr.MentorId == mentorId).ToListAsync();
    }

    public async Task<MentorshipRequests> CreateMentorshipRequest(MentorshipRequests mentorshipRequest)
    {
        await MentorshipRequests.AddAsync(mentorshipRequest);
        await _context.SaveChangesAsync();
        return mentorshipRequest;
    }

    public async Task<MentorshipRequests> UpdateMentorshipRequest(Guid id, MentorshipRequests updateMentorshipRequest)
    {
        var currentMentorshipRequest = await GetMentorshipRequestById(id);
        currentMentorshipRequest.Status = updateMentorshipRequest.Status;
        currentMentorshipRequest.ScheduledAt = updateMentorshipRequest.ScheduledAt;
        await _context.SaveChangesAsync();
        return currentMentorshipRequest;
    }

    public async Task DeleteMentorshipRequest(Guid id)
    {
        var deleteMentorshipRequest = await GetMentorshipRequestById(id);
        MentorshipRequests.Remove(deleteMentorshipRequest);
        await _context.SaveChangesAsync();
    }
}
