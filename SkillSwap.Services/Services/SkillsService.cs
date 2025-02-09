using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Services.Services;

public class SkillsService : ISkills
{
    private readonly SkillSwapDbContext _context;
    private DbSet<Skills> Skills => _context.Skills;

    public SkillsService(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<List<Skills>> GetAllSkills()
    {
        return await Skills.AsNoTracking().ToListAsync();
    }

    public async Task<Skills> GetSkillById(Guid id)
    {
        var skill = await Skills.FirstOrDefaultAsync(s => s.Id == id);

        if (skill == null) ErrorHelper.ThrowNotFoundException("Skill not found.");

        return skill;
    }

    public async Task<Skills> CreateSkill(Skills skill)
    {
        await EnsureSkillNameIsUnique(skill.Name);

        if (string.IsNullOrEmpty(skill.Name) || string.IsNullOrEmpty(skill.Description))
            ErrorHelper.ThrowBadRequestException("Name and description are required.");

        await Skills.AddAsync(skill);
        await _context.SaveChangesAsync();

        return skill;
    }

    public async Task<Skills> UpdateSkill(Guid id, Skills updateSkill)
    {
        var currentSkill = await GetSkillById(id);

        await EnsureSkillNameIsUnique(updateSkill.Name, id);

        currentSkill.Name = updateSkill.Name;
        currentSkill.Description = updateSkill.Description;

        await _context.SaveChangesAsync();

        return currentSkill;
    }

    public async Task DeleteSkill(Guid id)
    {
        var deleteSkill = await GetSkillById(id);

        Skills.Remove(deleteSkill);
        await _context.SaveChangesAsync();
    }

    #region Private Methods

    private async Task EnsureSkillNameIsUnique(string name, Guid? updatingSkillId = null)
    {
        bool skillExists = await Skills.AnyAsync(s => s.Name == name && (!updatingSkillId.HasValue || s.Id != updatingSkillId));

        if (skillExists) ErrorHelper.ThrowConflictException("Skill Name already exists.");
    }

    #endregion
}
