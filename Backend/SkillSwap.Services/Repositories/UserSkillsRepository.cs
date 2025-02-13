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
        var user = await Users.AsNoTracking().Include(u => u.Skills)
                              .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null || user.Skills == null) return new List<Skills>();

        return user.Skills.ToList();
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
        var user = await Users.Include(u => u.Skills).FirstOrDefaultAsync(u => u.Id == userId);

        if (user != null && user.Skills == null) user.Skills = new List<Skills>();

        user?.Skills.Add(skill);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveSkillFromUser(Guid userId, Skills skill)
    {
        var user = await Users.Include(u => u.Skills).FirstOrDefaultAsync(u => u.Id == userId);
        user.Skills.Remove(skill);
        await _context.SaveChangesAsync();
    }
}
