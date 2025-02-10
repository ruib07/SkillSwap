using SkillSwap.Entities.Entities;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;

namespace SkillSwap.Services.Services;

public class MentorshipRequestsService
{
    private readonly IMentorshipRequestsRepository _mentorshipRequestsRepository;

    public MentorshipRequestsService(IMentorshipRequestsRepository mentorshipRequestsRepository)
    {
        _mentorshipRequestsRepository = mentorshipRequestsRepository;
    }

    public async Task<MentorshipRequests> GetMentorshipRequestById(Guid id)
    {
        var mentorshipRequest = await _mentorshipRequestsRepository.GetMentorshipRequestById(id);

        if (mentorshipRequest == null) ErrorHelper.ThrowNotFoundException("Mentorship Request not found.");

        return mentorshipRequest;
    }

    public async Task<List<MentorshipRequests>> GetMentorshipRequestsbyLearnerId(Guid learnerId)
    {
        var mentorshipRequestsByLearner = await _mentorshipRequestsRepository.GetMentorshipRequestsbyLearnerId(learnerId);

        if (mentorshipRequestsByLearner == null || mentorshipRequestsByLearner.Count == 0)
            ErrorHelper.ThrowNotFoundException("No mentorship requests found to that learner.");

        return mentorshipRequestsByLearner;
    }

    public async Task<List<MentorshipRequests>> GetMentorshipRequestsbyMentorId(Guid mentorId)
    {
        var mentorshipRequestsByMentor = await _mentorshipRequestsRepository.GetMentorshipRequestsbyMentorId(mentorId);

        if (mentorshipRequestsByMentor == null || mentorshipRequestsByMentor.Count == 0)
            ErrorHelper.ThrowNotFoundException("No mentorship requests found to that mentor.");

        return mentorshipRequestsByMentor;
    }

    public async Task<MentorshipRequests> CreateMentorshipRequest(MentorshipRequests mentorshipRequest)
    {
        return await _mentorshipRequestsRepository.CreateMentorshipRequest(mentorshipRequest);
    }

    public async Task<MentorshipRequests> UpdateMentorshipRequest(Guid id, MentorshipRequests updateMentorshipRequest)
    {
        return await _mentorshipRequestsRepository.UpdateMentorshipRequest(id, updateMentorshipRequest);
    }

    public async Task DeleteMentorshipRequest(Guid id)
    {
        await _mentorshipRequestsRepository.DeleteMentorshipRequest(id);
    }
}
