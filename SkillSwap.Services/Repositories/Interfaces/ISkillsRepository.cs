using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Repositories.Interfaces;

public interface ISkillsRepository
{
    Task<List<Skills>> GetAllSkills();
    Task<Skills> GetSkillById(Guid id);
    Task<Skills> CreateSkill(Skills skill);
    Task<Skills> UpdateSkill(Guid id, Skills updateSkill);
    Task DeleteSkill(Guid id);

    Task<bool> EnsureSkillNameIsUnique(string name, Guid? updatingSkillId = null);
}
