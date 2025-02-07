using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Services.Services;

public class SkillsService : ISkills
{
    private readonly SkillSwapDbContext _context;

    public SkillsService(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<List<Skills>> GetAllSkills()
    {
        return await _context.Skills.AsNoTracking().ToListAsync();
    }

    public async Task<Skills> GetSkillById(Guid id)
    {
        var skill = await _context.Skills.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == id);

        if (skill == null) ErrorHelper.ThrowNotFoundException("Skill not found.");

        return skill;
    }

    public async Task<Skills> CreateSkill(Skills skill)
    {
        var existingSkillName = await _context.Skills.FirstOrDefaultAsync(esn => esn.Name == skill.Name);

        if (string.IsNullOrEmpty(skill.Name) || string.IsNullOrEmpty(skill.Description))
            ErrorHelper.ThrowBadRequestException("Name and description are required.");

        if (existingSkillName != null) ErrorHelper.ThrowConflictException("Skill Name already exists.");

        await _context.Skills.AddAsync(skill);
        await _context.SaveChangesAsync();

        return skill;
    }

    public async Task<Skills> UpdateSkill(Guid id, Skills updateSkill)
    {
        var currentSkill = await _context.Skills.FirstOrDefaultAsync(cs => cs.Id == id);

        if (currentSkill == null) ErrorHelper.ThrowNotFoundException("Skill not found.");

        var existingSkillName = await _context.Skills.FirstOrDefaultAsync(esn => esn.Name == updateSkill.Name);

        if (existingSkillName != null) ErrorHelper.ThrowConflictException("Skill Name already exists.");

        currentSkill.Name = updateSkill.Name;
        currentSkill.Description = updateSkill.Description;

        await _context.SaveChangesAsync();

        return currentSkill;
    }

    public async Task DeleteSkill(Guid id)
    {
        var deleteSkill = await _context.Skills.FirstOrDefaultAsync(ds => ds.Id == id);

        if (deleteSkill == null) ErrorHelper.ThrowNotFoundException("Skill not found.");

        _context.Skills.Remove(deleteSkill);
        await _context.SaveChangesAsync();
    }
}
