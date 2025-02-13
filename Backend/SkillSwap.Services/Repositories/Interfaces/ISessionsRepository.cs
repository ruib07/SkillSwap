using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Repositories.Interfaces;

public interface ISessionsRepository
{
    Task<Sessions> GetSessionById(Guid id);
    Task<List<Sessions>> GetSessionsByMentorshipRequestId(Guid mentorshipRequestId);
    Task<Sessions> CreateSession(Sessions session);
    Task UpdateSession(Sessions session);
    Task DeleteSession(Guid id);
}
