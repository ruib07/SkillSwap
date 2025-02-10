using SkillSwap.Entities.Entities;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;

namespace SkillSwap.Services.Services;

public class SkillsService
{
    private readonly ISkillsRepository _skillsRepository;

    public SkillsService(ISkillsRepository skillsRepository)
    {
        _skillsRepository = skillsRepository;
    }

    public async Task<List<Skills>> GetAllSkills()
    {
        return await _skillsRepository.GetAllSkills();
    }

    public async Task<Skills> GetSkillById(Guid id)
    {
        var skill = await _skillsRepository.GetSkillById(id);

        if (skill == null) ErrorHelper.ThrowNotFoundException("Skill not found.");

        return skill;
    }

    public async Task<Skills> CreateSkill(Skills skill)
    {
        await ValidateSkill(skill);

        if (string.IsNullOrEmpty(skill.Name) || string.IsNullOrEmpty(skill.Description))
            ErrorHelper.ThrowBadRequestException("Name and description are required.");

        return await _skillsRepository.CreateSkill(skill);
    }

    public async Task<Skills> UpdateSkill(Guid id, Skills updateSkill)
    {
        var currentSkill = await GetSkillById(id);

        await ValidateSkill(updateSkill, id);

        currentSkill.Name = updateSkill.Name;
        currentSkill.Description = updateSkill.Description;

        await _skillsRepository.UpdateSkill(currentSkill);
        return currentSkill;
    }

    public async Task DeleteSkill(Guid id)
    {
        await _skillsRepository.DeleteSkill(id);
    }

    #region Private Methods

    private async Task ValidateSkill(Skills skill, Guid? updatingSkillId = null)
    {
        if (string.IsNullOrEmpty(skill.Name) || string.IsNullOrEmpty(skill.Description))
        {
            ErrorHelper.ThrowBadRequestException("Name and description are required.");
        }

        var skillExists = await _skillsRepository.EnsureSkillNameIsUnique(skill.Name, updatingSkillId);

        if (skillExists) ErrorHelper.ThrowConflictException("Skill Name already exists.");
    }

    #endregion
}
