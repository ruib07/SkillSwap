using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Repositories.Interfaces;

namespace SkillSwap.Services.Repositories;

public class SkillsRepository : ISkillsRepository
{
    private readonly SkillSwapDbContext _context;
    private DbSet<Skills> Skills => _context.Skills;

    public SkillsRepository(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<List<Skills>> GetAllSkills()
    {
        return await Skills.AsNoTracking().ToListAsync();
    }

    public async Task<Skills> GetSkillById(Guid id)
    {
        return await Skills.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Skills> CreateSkill(Skills skill)
    {
        await Skills.AddAsync(skill);
        await _context.SaveChangesAsync();

        return skill;
    }

    public async Task UpdateSkill(Skills skill)
    {
        _context.Skills.Update(skill);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSkill(Guid id)
    {
        var deleteSkill = await GetSkillById(id);

        Skills.Remove(deleteSkill);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> EnsureSkillNameIsUnique(string name, Guid? updatingSkillId = null)
    {
        return await Skills.AnyAsync(s => s.Name == name && (!updatingSkillId.HasValue || s.Id != updatingSkillId));
    }
}
