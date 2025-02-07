using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Interfaces;

public interface IPayments
{
    Task<Payments> GetPaymentById(Guid id);
    Task<Payments> GetPaymentsByPayerId(Guid payerId);
    Task<Payments> GetPaymentsByMentorId(Guid mentorId);
    Task<Payments> SendPayment(Payments payment);
}
