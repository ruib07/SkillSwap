using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Interfaces;

public interface ISessions
{
    Task<Sessions> GetSessionById(Guid id);
    Task<Sessions> GetSessionsByMentorshipRequestId(Guid mentorshipRequestId);
    Task<Sessions> CreateSession(Sessions session);
    Task<Sessions> UpdateSession(Guid id, Sessions updateSession);
    Task DeleteSession(Guid id);
}
