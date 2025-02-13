using SkillSwap.Entities.Entities;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;

namespace SkillSwap.Services.Services;

public class UsersService
{
    private readonly IUsersRepository _usersRepository;

    public UsersService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<Users> GetUserById(Guid id)
    {
        var user = await _usersRepository.GetUserById(id);
        if (user == null) ErrorHelper.ThrowNotFoundException("User not found.");
        return user;
    }

    public async Task<Users> CreateUser(Users user)
    {
        var existingUser = await _usersRepository.GetUserByEmail(user.Email);

        if (existingUser != null)
            ErrorHelper.ThrowConflictException("Email already in use.");

        if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
            ErrorHelper.ThrowBadRequestException("Name, email, and password are required.");

        if (user.Balance < 0)
            ErrorHelper.ThrowBadRequestException("Balance cannot be negative.");

        user.Password = PasswordHasher.HashPassword(user.Password);

        return await _usersRepository.CreateUser(user);
    }

    public async Task<string> GeneratePasswordResetToken(string email)
    {
        var user = await _usersRepository.GetUserByEmail(email);

        if (user != null) return await _usersRepository.GeneratePasswordResetToken(user.Id);

        return null;
    }

    public async Task UpdatePassword(string token, string newPassword, string confirmNewPassword)
    {
        if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmNewPassword))
            ErrorHelper.ThrowBadRequestException("Password fields cannot be empty.");

        if (newPassword != confirmNewPassword)
            ErrorHelper.ThrowBadRequestException("Passwords do not match.");

        var passwordResetToken = await _usersRepository.GetPasswordResetToken(token);
        if (passwordResetToken == null)
            ErrorHelper.ThrowBadRequestException("Invalid or expired token.");

        var user = passwordResetToken.User;
        user.Password = PasswordHasher.HashPassword(newPassword);

        await _usersRepository.UpdateUser(user);
        await _usersRepository.RemovePasswordResetToken(passwordResetToken);
    }

    public async Task<Users> UpdateUser(Guid id, Users updateUser)
    {
        var currentUser = await GetUserById(id);

        var emailExists = await _usersRepository.GetUserByEmail(updateUser.Email);
        if (emailExists != null && emailExists.Id != id)
            ErrorHelper.ThrowConflictException("Email already in use.");

        currentUser.Name = updateUser.Name;
        currentUser.Email = updateUser.Email;
        currentUser.Password = PasswordHasher.HashPassword(updateUser.Password);
        currentUser.Bio = updateUser.Bio;
        currentUser.ProfilePicture = updateUser.ProfilePicture;
        currentUser.Balance = updateUser.Balance;

        await _usersRepository.UpdateUser(currentUser);
        return currentUser;
    }

    public async Task<decimal> UpdateBalance(Guid id, decimal userBalance)
    {
        var user = await GetUserById(id);
        if (userBalance < 0)
            ErrorHelper.ThrowBadRequestException("Balance cannot be negative.");

        user.Balance = userBalance;
        await _usersRepository.UpdateUser(user); 

        return userBalance;
    }

    public async Task DeleteUser(Guid id)
    {
        await _usersRepository.DeleteUser(id);
    }
}
