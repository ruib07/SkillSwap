using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Repositories.Interfaces;

public interface IMentorshipRequestsRepository
{
    Task<MentorshipRequests> GetMentorshipRequestById(Guid id);
    Task<List<MentorshipRequests>> GetMentorshipRequestsbyLearnerId(Guid learnerId);
    Task<List<MentorshipRequests>> GetMentorshipRequestsbyMentorId(Guid mentorId);
    Task<MentorshipRequests> CreateMentorshipRequest(MentorshipRequests mentorshipRequest);
    Task UpdateMentorshipRequest(MentorshipRequests mentorshipRequest);
    Task DeleteMentorshipRequest(Guid id);
}
