using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Interfaces;

public interface ISkills
{
    Task<List<Skills>> GetAllSkills();
    Task<Skills> GetSkillById(Guid id);
    Task<Skills> CreateSkill(Skills skill);
    Task<Skills> UpdateSkill(Guid id, Skills updateSkill);
    Task DeleteSkill(Guid id);
}
