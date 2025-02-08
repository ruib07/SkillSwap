﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Constants;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Server.Controllers;

[Route("payments")]
[ApiController]
public class PaymentsController : ControllerBase
{
    private readonly IPayments _payments;

    public PaymentsController(IPayments payments)
    {
        _payments = payments;
    }

    // GET payments/{paymentId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("{paymentId}")]
    public async Task<IActionResult> GetPaymentById(Guid paymentId)
    {
        var payment = await _payments.GetPaymentById(paymentId);

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
    public async Task<ActionResult<Reviews>> SendPayment([FromBody] Payments payment)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _payments.SendPayment(payment);

        return StatusCode(StatusCodes.Status201Created, "Payment created successffully.");
    }
}
