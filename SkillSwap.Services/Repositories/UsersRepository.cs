using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Repositories.Interfaces;

namespace SkillSwap.Services.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly SkillSwapDbContext _context;
    private DbSet<Users> Users => _context.Users;
    private DbSet<PasswordResetToken> PasswordResetTokens => _context.PasswordResetsToken;

    public UsersRepository(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<Users> GetUserById(Guid id)
    {
        return await Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Users> GetUserByEmail(string email)
    {
        return await Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Users> CreateUser(Users user)
    {
        await Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<string> GeneratePasswordResetToken(Guid userId)
    {
        var token = Guid.NewGuid().ToString();
        var expiryDate = DateTime.UtcNow.AddHours(1);

        var passwordResetToken = new PasswordResetToken()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiryDate = expiryDate
        };

        await PasswordResetTokens.AddAsync(passwordResetToken);
        await _context.SaveChangesAsync();

        return token;
    }

    public async Task<PasswordResetToken> GetPasswordResetToken(string token)
    {
        return await PasswordResetTokens.Include(prt => prt.User)
                                        .FirstOrDefaultAsync(prt => prt.Token == token && prt.ExpiryDate > DateTime.UtcNow);
    }

    public async Task RemovePasswordResetToken(PasswordResetToken token)
    {
        PasswordResetTokens.Remove(token);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUser(Users user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> UpdateBalance(Guid id, decimal userBalance)
    {
        var user = await GetUserById(id);
        user.Balance = userBalance;
        await _context.SaveChangesAsync();

        return userBalance;
    }

    public async Task DeleteUser(Guid id)
    {
        var user = await GetUserById(id);
        Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}
