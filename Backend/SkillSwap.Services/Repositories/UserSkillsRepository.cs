using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Repositories.Interfaces;

namespace SkillSwap.Services.Repositories;

public class UserSkillsRepository : IUserSkillsRepository
{
    private readonly SkillSwapDbContext _context;
    private DbSet<Users> Users => _context.Users;
    private DbSet<Skills> Skills => _context.Skills;

    public UserSkillsRepository(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<List<Skills>> GetUserSkillsByUser(Guid userId)
    {
        var skills = await _context.UserSkills.Where(us => us.UserId == userId)
                                              .Select(us => us.Skill) .AsNoTracking()
                                              .ToListAsync();

        return skills;
    }

    public async Task<bool> UserHasSkill(Guid userId, Guid skillId)
    {
        return await _context.UserSkills.AnyAsync(us => us.UserId == userId && us.SkillId == skillId);
    }

    public async Task<bool> UserExists(Guid userId)
    {
        return await Users.AnyAsync(u => u.Id == userId);
    }

    public async Task<bool> SkillExists(Guid skillId)
    {
        return await Skills.AnyAsync(s => s.Id == skillId);
    }

    public async Task AddSkillToUser(Guid userId, Skills skill)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        var skillExists = await _context.Skills.AnyAsync(s => s.Id == skill.Id);

        if (!userExists || !skillExists) return;

        var userSkill = new UserSkills
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SkillId = skill.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.UserSkills.Add(userSkill);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveSkillFromUser(Guid userId, Skills skill)
    {
        var userSkill = await _context.UserSkills
            .FirstOrDefaultAsync(us => us.UserId == userId && us.SkillId == skill.Id);

        if (userSkill != null)
        {
            _context.UserSkills.Remove(userSkill);
            await _context.SaveChangesAsync();
        }
    }
}
