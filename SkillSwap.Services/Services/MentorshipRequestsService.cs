using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Services.Services;

public class MentorshipRequestsService : IMentorshipRequests
{
    private readonly SkillSwapDbContext _context;

    public MentorshipRequestsService(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<MentorshipRequests> GetMentorshipRequestById(Guid id)
    {
        var mentorshipRequest = await _context.MentorshipRequests.AsNoTracking()
                                .FirstOrDefaultAsync(mr => mr.Id == id);

        if (mentorshipRequest == null) ErrorHelper.ThrowNotFoundException("Mentorship Request not found.");

        return mentorshipRequest;
    }

    public async Task<List<MentorshipRequests>> GetMentorshipRequestsbyLearnerId(Guid learnerId)
    {
        var mentorshipRequestsByLearner = await _context.MentorshipRequests.AsNoTracking()
                                 .Where(mr => mr.LearnerId == learnerId).ToListAsync();

        if (mentorshipRequestsByLearner == null || mentorshipRequestsByLearner.Count == 0)
            ErrorHelper.ThrowNotFoundException("No mentorship requests found to that learner.");

        return mentorshipRequestsByLearner;
    }

    public async Task<List<MentorshipRequests>> GetMentorshipRequestsbyMentorId(Guid mentorId)
    {
        var mentorshipRequestsByMentor = await _context.MentorshipRequests.AsNoTracking()
                                .Where(mr => mr.MentorId == mentorId).ToListAsync();

        if (mentorshipRequestsByMentor == null || mentorshipRequestsByMentor.Count == 0)
            ErrorHelper.ThrowNotFoundException("No mentorship requests found to that mentor.");

        return mentorshipRequestsByMentor;
    }

    public async Task<MentorshipRequests> CreateMentorshipRequest(MentorshipRequests mentorshipRequest)
    {
        await _context.MentorshipRequests.AddAsync(mentorshipRequest);
        await _context.SaveChangesAsync();

        return mentorshipRequest;
    }

    public async Task<MentorshipRequests> UpdateMentorshipRequest(Guid id, MentorshipRequests updateMentorshipRequest)
    {
        var currentMentorshipRequest = await _context.MentorshipRequests.FirstOrDefaultAsync(cmr => cmr.Id == id);

        if (currentMentorshipRequest == null) ErrorHelper.ThrowNotFoundException("Mentorship request not found.");

        currentMentorshipRequest.Status = updateMentorshipRequest.Status;
        currentMentorshipRequest.ScheduledAt = updateMentorshipRequest.ScheduledAt;

        await _context.SaveChangesAsync();

        return currentMentorshipRequest;
    }

    public async Task DeleteMentorshipRequest(Guid id)
    {
        var deleteMentorshipRequest = await _context.MentorshipRequests.FirstOrDefaultAsync(dmr => dmr.Id == id);

        if (deleteMentorshipRequest == null) ErrorHelper.ThrowNotFoundException("Mentorship request not found.");

        _context.MentorshipRequests.Remove(deleteMentorshipRequest);
        await _context.SaveChangesAsync();
    }
}
