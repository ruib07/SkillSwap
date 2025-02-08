using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Interfaces;

public interface IUsers
{
    Task<Users> GetUserById(Guid id);
    Task<Users> CreateUser(Users user);
    Task<string> GeneratePasswordResetToken(string email);
    Task UpdatePassword(string token, string newPassword, string confirmNewPassword);
    Task<Users> UpdateUser(Guid id, Users updateUser);
    Task<Users> UpdateBalance(Guid id, decimal userBalance);
    Task DeleteUser(Guid id);
}
