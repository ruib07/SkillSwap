using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Interfaces;

public interface IUserSkills
{
    Task<UserSkills> GetUserSkillsByUserId(Guid userId);
    Task<UserSkills> CreateUserSkill(UserSkills userSkill);
    Task DeleteSkill(Guid id);
}
