using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Services.Services;

public class UsersService : IUsers
{
    private readonly SkillSwapDbContext _context;

    public UsersService(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<Users> GetUserById(Guid id)
    {
        var user = await _context.Users.AsNoTracking()
                   .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) ErrorHelper.ThrowNotFoundException("User not found.");

        return user;
    }

    public async Task<Users> CreateUser(Users user)
    {
        var existingEmail = await _context.Users.FirstOrDefaultAsync(eu => eu.Email == user.Email);

        if (existingEmail != null)
        {
            ErrorHelper.ThrowConflictException("Email already in use.");
        }

        if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email)
            || string.IsNullOrWhiteSpace(user.Password))
            ErrorHelper.ThrowBadRequestException("Name, email, and password are required.");

        if (user.Balance < 0) ErrorHelper.ThrowBadRequestException("Balance cannot be negative.");

        user.Password = PasswordHasher.HashPassword(user.Password);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<string> GeneratePasswordResetToken(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        var token = Guid.NewGuid().ToString();
        var expiryDate = DateTime.UtcNow.AddHours(1);

        var passwordResetToken = new PasswordResetToken()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = token,
            ExpiryDate = expiryDate
        };

        await _context.PasswordResetsToken.AddAsync(passwordResetToken);
        await _context.SaveChangesAsync();

        return token;
    }

    public async Task UpdatePassword(string token, string newPassword, string confirmNewPassword)
    {
        if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmNewPassword))
            ErrorHelper.ThrowBadRequestException("Password fields cannot be empty.");


        if (newPassword != confirmNewPassword) 
            ErrorHelper.ThrowBadRequestException("The password do not match.");


        var passwordResetToken = await _context.PasswordResetsToken.Include(prt => prt.User)
                                 .FirstOrDefaultAsync(prt => prt.Token == token && prt.ExpiryDate > DateTime.UtcNow);

        if (passwordResetToken == null) ErrorHelper.ThrowBadRequestException("Invalid or expired token.");

        var user = passwordResetToken.User;
        user.Password = PasswordHasher.HashPassword(newPassword);

        _context.PasswordResetsToken.Remove(passwordResetToken);
        await _context.SaveChangesAsync();
    }

    public async Task<Users> UpdateUser(Guid id, Users updateUser)
    {
        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (currentUser == null) ErrorHelper.ThrowNotFoundException("User not found.");

        bool emailExists = await _context.Users.AnyAsync(ee => ee.Email == updateUser.Email && ee.Id != currentUser.Id);

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

    public async Task<Users> UpdateUserBalance(Guid id, decimal userBalance)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) ErrorHelper.ThrowNotFoundException("User not found.");

        if (userBalance < 0) ErrorHelper.ThrowBadRequestException("Balance cannot be negative.");

        user.Balance = userBalance;

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task DeleteUser(Guid id)
    {
        var deleteUser = await _context.Users.FirstOrDefaultAsync(du => du.Id == id);

        if (deleteUser == null) ErrorHelper.ThrowNotFoundException("User not found.");

        _context.Users.Remove(deleteUser);
        await _context.SaveChangesAsync();
    }
}
