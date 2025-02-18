using SkillSwap.Entities.Entities;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;

namespace SkillSwap.Services.Services;

public class UserSkillsService
{
    private readonly IUserSkillsRepository _userSkillsRepository;
    private readonly ISkillsRepository _skillsRepository;

    public UserSkillsService(IUserSkillsRepository userSkillsRepository, ISkillsRepository skillsRepository)
    {
        _userSkillsRepository = userSkillsRepository;
        _skillsRepository = skillsRepository;
    }

    public async Task<List<Skills>> GetUserSkillsByUser(Guid userId)
    {
        return await _userSkillsRepository.GetUserSkillsByUser(userId);
    }

    public async Task<bool> UserHasSkill(Guid userId, Guid skillId)
    {
        return await _userSkillsRepository.UserHasSkill(userId, skillId);
    }

    public async Task CreateUserSkill(Guid userId, Guid skillId)
    {
        if (!await _userSkillsRepository.UserExists(userId))
            ErrorHelper.ThrowNotFoundException("User not found.");

        if (!await _userSkillsRepository.SkillExists(skillId))
            ErrorHelper.ThrowNotFoundException("Skill not found.");

        if (await _userSkillsRepository.UserHasSkill(userId, skillId))
            ErrorHelper.ThrowConflictException("User already has this skill.");

        var skill = await _skillsRepository.GetSkillById(skillId);

        if (skill == null)
            ErrorHelper.ThrowNotFoundException("Skill not found in repository.");

        await _userSkillsRepository.AddSkillToUser(userId, skill);
    }

    public async Task DeleteUserSkill(Guid userId, Guid skillId)
    {
        if (!await _userSkillsRepository.UserExists(userId))
            ErrorHelper.ThrowNotFoundException("User not found.");

        var skill = await _skillsRepository.GetSkillById(skillId);

        if (skill == null)
            ErrorHelper.ThrowNotFoundException("Skill not found in repository.");

        await _userSkillsRepository.RemoveSkillFromUser(userId, skill);
    }
}