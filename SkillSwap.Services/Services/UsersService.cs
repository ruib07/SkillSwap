using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Services.Services;

public class UsersService : IUsers
{
    private readonly SkillSwapDbContext _context;
    private DbSet<Users> Users => _context.Users;
    private DbSet<PasswordResetToken> PasswordResetToken => _context.PasswordResetsToken;

    public UsersService(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<Users> GetUserById(Guid id)
    {
        var user = await Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) ErrorHelper.ThrowNotFoundException("User not found.");

        return user;
    }

    public async Task<Users> CreateUser(Users user)
    {
        var existingEmail = await Users.FirstOrDefaultAsync(eu => eu.Email == user.Email);

        if (existingEmail != null)
        {
            ErrorHelper.ThrowConflictException("Email already in use.");
        }

        if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email)
            || string.IsNullOrWhiteSpace(user.Password))
            ErrorHelper.ThrowBadRequestException("Name, email, and password are required.");

        if (user.Balance < 0) ErrorHelper.ThrowBadRequestException("Balance cannot be negative.");

        user.Password = PasswordHasher.HashPassword(user.Password);

        await Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<string> GeneratePasswordResetToken(string email)
    {
        var user = await Users.FirstOrDefaultAsync(u => u.Email == email);
        var token = Guid.NewGuid().ToString();
        var expiryDate = DateTime.UtcNow.AddHours(1);

        if (user != null)
        {
            var passwordResetToken = new PasswordResetToken()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = token,
                ExpiryDate = expiryDate
            };

            await PasswordResetToken.AddAsync(passwordResetToken);
            await _context.SaveChangesAsync();
        }

        return token;
    }

    public async Task UpdatePassword(string token, string newPassword, string confirmNewPassword)
    {
        if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmNewPassword))
            ErrorHelper.ThrowBadRequestException("Password fields cannot be empty.");

        if (newPassword != confirmNewPassword) 
            ErrorHelper.ThrowBadRequestException("Passwords do not match.");

        var passwordResetToken = await PasswordResetToken.Include(prt => prt.User)
                                 .FirstOrDefaultAsync(prt => prt.Token == token && prt.ExpiryDate > DateTime.UtcNow);

        if (passwordResetToken == null) ErrorHelper.ThrowBadRequestException("Invalid or expired token.");

        var user = passwordResetToken.User;
        user.Password = PasswordHasher.HashPassword(newPassword);

        PasswordResetToken.Remove(passwordResetToken);
        await _context.SaveChangesAsync();
    }

    public async Task<Users> UpdateUser(Guid id, Users updateUser)
    {
        var currentUser = await GetUserById(id);

        bool emailExists = await Users.AnyAsync(ee => ee.Email == updateUser.Email && ee.Id != currentUser.Id);

        if (emailExists) ErrorHelper.ThrowConflictException("Email already in use.");

        currentUser.Name = updateUser.Name;
        currentUser.Email = updateUser.Email;
        currentUser.Password = PasswordHasher.HashPassword(updateUser.Password);
        currentUser.Bio = updateUser.Bio;
        currentUser.ProfilePicture = updateUser.ProfilePicture;
        currentUser.Balance = updateUser.Balance;

        await _context.SaveChangesAsync();

        return currentUser;
    }

    public async Task<Users> UpdateBalance(Guid id, decimal userBalance)
    {
        var user = await GetUserById(id);

        if (userBalance < 0) ErrorHelper.ThrowBadRequestException("Balance cannot be negative.");

        user.Balance = userBalance;

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task DeleteUser(Guid id)
    {
        var deleteUser = await GetUserById(id);

        Users.Remove(deleteUser);
        await _context.SaveChangesAsync();
    }
}
