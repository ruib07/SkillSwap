﻿using SkillSwap.Entities.Entities;

namespace SkillSwap.Server.Models;

public class Responses
{
    public class CreationResponse
    {
        public string Message { get; set; }
        public Guid Id { get; set; }
    }

    public class UpdatePaymentStatusResponse
    {
        public string Message { get; set; }
        public PaymentStatus UpdatedStatus { get; set; }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}
