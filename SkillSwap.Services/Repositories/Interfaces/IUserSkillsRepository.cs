using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Repositories.Interfaces;

public interface IUserSkillsRepository
{
    Task<List<Skills>> GetUserSkillsByUser(Guid userId);
    Task<bool> UserExists(Guid userId);
    Task<bool> SkillExists(Guid skillId);
    Task AddSkillToUser(Guid userId, Skills skill);
    Task RemoveSkillFromUser(Guid userId, Skills skill);
}
