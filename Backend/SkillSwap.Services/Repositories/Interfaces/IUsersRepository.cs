using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Repositories.Interfaces;

public interface IUsersRepository
{
    Task<List<Users>> GetUsers();
    Task<Users> GetUserById(Guid id);
    Task<Users> GetUserByEmail(string email);
    Task<List<Users>> GetMentors();
    Task<Users> CreateUser(Users user);
    Task<string> GeneratePasswordResetToken(Guid userId);
    Task<PasswordResetToken> GetPasswordResetToken(string token);
    Task RemovePasswordResetToken(PasswordResetToken token);
    Task UpdateUser(Users user);
    Task<decimal> UpdateBalance(Users user, decimal userBalance);
    Task DeleteUser(Guid id);
}
