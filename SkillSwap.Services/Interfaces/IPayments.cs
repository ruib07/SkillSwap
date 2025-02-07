using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Interfaces;

public interface IPayments
{
    Task<Payments> GetPaymentById(Guid id);
    Task<List<Payments>> GetPaymentsByPayerId(Guid payerId);
    Task<List<Payments>> GetPaymentsByMentorId(Guid mentorId);
    Task<Payments> SendPayment(Payments payment);
}
