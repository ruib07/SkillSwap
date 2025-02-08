using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Services.Services;

public class UserSkillsService : IUserSkills
{
    private readonly SkillSwapDbContext _context;

    public UserSkillsService(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<List<Skills>> GetUserSkillsByUser(Guid userId)
    {
        var user = await _context.Users.AsNoTracking().Include(u => u.Skills)
                   .FirstOrDefaultAsync(u => u.Id == userId);

        return user?.Skills.ToList() ?? new List<Skills>();
    }

    public async Task CreateUserSkill(Guid userId, Guid skillId)
    {
        var user = await _context.Users.Include(u => u.Skills).FirstOrDefaultAsync(u => u.Id == userId);
        var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == skillId);

        if (user == null) ErrorHelper.ThrowNotFoundException("User not found.");
        if (skill == null) ErrorHelper.ThrowNotFoundException("Skill not found.");
        if (user.Skills.Any(s => s.Id == skillId)) ErrorHelper.ThrowConflictException("User already has this skill.");

        user.Skills.Add(skill);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserSkill(Guid userId, Guid skillId)
    {
        var user = await _context.Users.Include(u => u.Skills).FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) ErrorHelper.ThrowNotFoundException("User not found.");

        var skill = user.Skills.FirstOrDefault(s => s.Id == skillId);

        if (skill == null) ErrorHelper.ThrowNotFoundException("User does not have this skill.");

        user.Skills.Remove(skill);
        await _context.SaveChangesAsync();
    }
}
