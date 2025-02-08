using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Interfaces;

public interface IUserSkills
{
    Task<List<Skills>> GetUserSkillsByUser(Guid userId);
    Task CreateUserSkill(Guid userId, Guid skillId);
    Task DeleteUserSkill(Guid userId, Guid skillId);
}
