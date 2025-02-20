using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Repositories.Interfaces;

namespace SkillSwap.Services.Repositories;

public class PaymentsRepository : IPaymentsRepository
{
    private readonly SkillSwapDbContext _context;
    private DbSet<Payments> Payments => _context.Payments;

    public PaymentsRepository(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<Payments> GetPaymentById(Guid id)
    {
        return await Payments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Payments>> GetPaymentsByPayerId(Guid payerId)
    {
        return await Payments.AsNoTracking().Where(pm => pm.PayerId == payerId).ToListAsync();
    }

    public async Task<List<Payments>> GetPaymentsByMentorId(Guid mentorId)
    {
        return await Payments.AsNoTracking().Where(pm => pm.MentorId == mentorId).ToListAsync();
    }

    public async Task<Payments> SendPayment(Payments payment)
    {
        await Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
        return payment;
    }

    public async Task<Payments> UpdatePaymentStatus(Guid id, PaymentStatus paymentStatus)
    {
        var existingPayment = await Payments.FirstOrDefaultAsync(p => p.Id == id);
        if (existingPayment == null) return null;

        existingPayment.Status = paymentStatus; 
        await _context.SaveChangesAsync();

        return existingPayment;
    }
}
