using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Services.Services;

public class PaymentsService : IPayments
{
    private readonly SkillSwapDbContext _context;

    public PaymentsService(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<Payments> GetPaymentById(Guid id)
    {
        var payment = await _context.Payments.AsNoTracking()
                      .FirstOrDefaultAsync(p => p.Id == id);

        if (payment == null) ErrorHelper.ThrowNotFoundException("Payment not found.");

        return payment;
    }

    public async Task<List<Payments>> GetPaymentsByPayerId(Guid payerId)
    {
        var paymentsByPayer = await _context.Payments.AsNoTracking()
                              .Where(pm => pm.PayerId == payerId).ToListAsync();

        if (paymentsByPayer == null || paymentsByPayer.Count == 0)
            ErrorHelper.ThrowNotFoundException("No payments found to that payer.");

        return paymentsByPayer;
    }

    public async Task<List<Payments>> GetPaymentsByMentorId(Guid mentorId)
    {
        var paymentsByMentor = await _context.Payments.AsNoTracking()
                               .Where(pm => pm.MentorId == mentorId).ToListAsync();

        if (paymentsByMentor == null || paymentsByMentor.Count == 0) 
            ErrorHelper.ThrowNotFoundException("No payments found to that mentor.");

        return paymentsByMentor;
    }

    public async Task<Payments> SendPayment(Payments payment)
    {
        if (payment.Amount <= 0)
            ErrorHelper.ThrowBadRequestException("Amount must be a positive number greater than zero.");

        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();

        return payment;
    }
}
