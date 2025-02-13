using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Repositories.Interfaces;

public interface IUsersRepository
{
    Task<Users> GetUserById(Guid id);
    Task<Users> GetUserByEmail(string email);
    Task<Users> CreateUser(Users user);
    Task<string> GeneratePasswordResetToken(Guid userId);
    Task<PasswordResetToken> GetPasswordResetToken(string token);
    Task RemovePasswordResetToken(PasswordResetToken token);
    Task UpdateUser(Users user);
    Task<decimal> UpdateBalance(Users user, decimal userBalance);
    Task DeleteUser(Guid id);
}
