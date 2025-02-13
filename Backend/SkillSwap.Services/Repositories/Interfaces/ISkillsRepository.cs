using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Repositories.Interfaces;

public interface ISkillsRepository
{
    Task<List<Skills>> GetAllSkills();
    Task<Skills> GetSkillById(Guid id);
    Task<Skills> CreateSkill(Skills skill);
    Task UpdateSkill(Skills skill);
    Task DeleteSkill(Guid id); 
    Task<bool> EnsureSkillNameIsUnique(string name, Guid? updatingSkillId = null);
}
