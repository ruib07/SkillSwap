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

    public async Task CreateUserSkill(Guid userId, Guid skillId)
    {
        if (!await _userSkillsRepository.UserExists(userId))
            ErrorHelper.ThrowNotFoundException("User not found.");

        if (!await _userSkillsRepository.SkillExists(skillId))
            ErrorHelper.ThrowNotFoundException("Skill not found.");

        var skills = await _userSkillsRepository.GetUserSkillsByUser(userId);
        if (skills.Any(s => s.Id == skillId))
            ErrorHelper.ThrowConflictException("User already has this skill.");

        var skill = await _skillsRepository.GetSkillById(skillId);
        await _userSkillsRepository.AddSkillToUser(userId, skill);
    }

    public async Task DeleteUserSkill(Guid userId, Guid skillId)
    {
        if (!await _userSkillsRepository.UserExists(userId))
            ErrorHelper.ThrowNotFoundException("User not found.");

        var skills = await _userSkillsRepository.GetUserSkillsByUser(userId);
        var skill = skills.FirstOrDefault(s => s.Id == skillId);

        if (skill == null)
            ErrorHelper.ThrowNotFoundException("User does not have this skill.");

        await _userSkillsRepository.RemoveSkillFromUser(userId, skill);
    }
}
