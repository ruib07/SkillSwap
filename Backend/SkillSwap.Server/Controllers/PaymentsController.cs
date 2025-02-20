using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Constants;
using SkillSwap.Services.Services;
using static SkillSwap.Server.Models.Responses;

namespace SkillSwap.Server.Controllers;

[Route("payments")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentsService _payments;

    public PaymentsController(PaymentsService payments)
    {
        _payments = payments;
    }

    // GET payments/{id}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById(Guid id)
    {
        var payment = await _payments.GetPaymentById(id);

        return Ok(payment);
    }

    // GET payments/bypayer/{payerId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("bypayer/{payerId}")]
    public async Task<ActionResult<List<Payments>>> GetPaymentsByPayerId(Guid payerId)
    {
        var paymentsByPayer = await _payments.GetPaymentsByPayerId(payerId);

        return Ok(paymentsByPayer);
    }

    // GET payments/bymentor/{mentorId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("bymentor/{mentorId}")]
    public async Task<ActionResult<List<Payments>>> GetPaymentsByMentorId(Guid mentorId)
    {
        var paymentsByMentor = await _payments.GetPaymentsByMentorId(mentorId);

        return Ok(paymentsByMentor);
    }

    // POST payments
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPost]
    public async Task<ActionResult<Payments>> SendPayment([FromBody] Payments payment)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _payments.SendPayment(payment);

        return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, new CreationResponse()
        {
            Message = "Payment created successfully.",
            Id = payment.Id
        });
    }

    // PATCH payments/{id}/status
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<Payments>> UpdatePaymentStatus(Guid id, [FromBody] PaymentStatus status)
    {
        var updatedPayment = await _payments.UpdatePaymentStatus(id, status);

        return Ok(new UpdatePaymentStatusResponse()
        {
            Message = "Payment status updated successfully.",
            UpdatedStatus = updatedPayment.Status
        });
    }
}
