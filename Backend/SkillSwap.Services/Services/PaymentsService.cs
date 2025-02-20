using SkillSwap.Entities.Entities;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;

namespace SkillSwap.Services.Services;

public class PaymentsService
{
    private readonly IPaymentsRepository _paymentsRepository;

    public PaymentsService(IPaymentsRepository paymentsRepository)
    {
        _paymentsRepository = paymentsRepository;
    }

    public async Task<Payments> GetPaymentById(Guid id)
    {
        var payment = await _paymentsRepository.GetPaymentById(id);

        if (payment == null) ErrorHelper.ThrowNotFoundException("Payment not found.");

        return payment;
    }

    public async Task<List<Payments>> GetPaymentsByPayerId(Guid payerId)
    {
        var paymentsByPayer = await _paymentsRepository.GetPaymentsByPayerId(payerId);

        if (paymentsByPayer == null || paymentsByPayer.Count == 0)
            ErrorHelper.ThrowNotFoundException("No payments found to that payer.");

        return paymentsByPayer;
    }

    public async Task<List<Payments>> GetPaymentsByMentorId(Guid mentorId)
    {
        var paymentsByMentor = await _paymentsRepository.GetPaymentsByMentorId(mentorId);

        if (paymentsByMentor == null || paymentsByMentor.Count == 0) 
            ErrorHelper.ThrowNotFoundException("No payments found to that mentor.");

        return paymentsByMentor;
    }

    public async Task<Payments> SendPayment(Payments payment)
    {
        if (payment.Amount <= 0) ErrorHelper.ThrowBadRequestException("Amount must be a positive number greater than zero.");

        return await _paymentsRepository.SendPayment(payment);
    }

    public async Task<Payments> UpdatePaymentStatus(Guid id, PaymentStatus paymentStatus)
    {
        var payment = await _paymentsRepository.GetPaymentById(id);

        if (payment == null) ErrorHelper.ThrowNotFoundException("Payment not found.");

        var updatedPayment = await _paymentsRepository.UpdatePaymentStatus(id, paymentStatus);

        if (updatedPayment == null)
            ErrorHelper.ThrowBadRequestException("Failed to update payment status.");

        return updatedPayment;
    }
}
