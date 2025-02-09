using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Services.Services;

public class MentorshipRequestsService : IMentorshipRequests
{
    private readonly SkillSwapDbContext _context;
    private DbSet<MentorshipRequests> MentorshipRequests => _context.MentorshipRequests;

    public MentorshipRequestsService(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<MentorshipRequests> GetMentorshipRequestById(Guid id)
    {
        var mentorshipRequest = await MentorshipRequests.FirstOrDefaultAsync(mr => mr.Id == id);

        if (mentorshipRequest == null) ErrorHelper.ThrowNotFoundException("Mentorship Request not found.");

        return mentorshipRequest;
    }

    public async Task<List<MentorshipRequests>> GetMentorshipRequestsbyLearnerId(Guid learnerId)
    {
        var mentorshipRequestsByLearner = await MentorshipRequests.Where(mr => mr.LearnerId == learnerId).ToListAsync();

        if (mentorshipRequestsByLearner == null || mentorshipRequestsByLearner.Count == 0)
            ErrorHelper.ThrowNotFoundException("No mentorship requests found to that learner.");

        return mentorshipRequestsByLearner;
    }

    public async Task<List<MentorshipRequests>> GetMentorshipRequestsbyMentorId(Guid mentorId)
    {
        var mentorshipRequestsByMentor = await MentorshipRequests.AsNoTracking().Where(mr => mr.MentorId == mentorId).ToListAsync();

        if (mentorshipRequestsByMentor == null || mentorshipRequestsByMentor.Count == 0)
            ErrorHelper.ThrowNotFoundException("No mentorship requests found to that mentor.");

        return mentorshipRequestsByMentor;
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
