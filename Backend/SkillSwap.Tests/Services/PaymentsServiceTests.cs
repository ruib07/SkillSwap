using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;
using System.Data;
using System.Net;

namespace SkillSwap.Tests.Services;

[TestFixture]
public class PaymentsServiceTests
{
    private Mock<IPaymentsRepository> paymentsRepositoryMock;
    private PaymentsService paymentsService;

    [SetUp]
    public void Setup()
    {
        paymentsRepositoryMock = new Mock<IPaymentsRepository>();
        paymentsService = new PaymentsService(paymentsRepositoryMock.Object);
    }

    [Test]
    public async Task GetPaymentById_RetunsPayment()
    {
        var payment = CreatePaymentsTemplate()[0];

        paymentsRepositoryMock.Setup(repo => repo.GetPaymentById(payment.Id)).ReturnsAsync(payment);

        var result = await paymentsService.GetPaymentById(payment.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(payment.Id));
            Assert.That(result.PayerId, Is.EqualTo(payment.PayerId));
            Assert.That(result.MentorId, Is.EqualTo(payment.MentorId));
            Assert.That(result.Amount, Is.EqualTo(payment.Amount));
            Assert.That(result.Status, Is.EqualTo(payment.Status));
        });
    }

    [Test]
    public void GetPaymentById_ReturnsNotFoundException_WhenPaymentIdDontExist()
    {
        paymentsRepositoryMock.Setup(repo => repo.GetPaymentById(It.IsAny<Guid>())).ReturnsAsync((Payments)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await paymentsService.GetPaymentById(Guid.NewGuid()));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("Payment not found."));
        });
    }

    [Test]
    public async Task GetPaymentsByPayerId_ReturnsPayments()
    {
        var payments = CreatePaymentsTemplate();

        paymentsRepositoryMock.Setup(repo => repo.GetPaymentsByPayerId(payments[0].PayerId)).ReturnsAsync(payments);

        var result = await paymentsService.GetPaymentsByPayerId(payments[0].PayerId);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(payments[0].Id));
            Assert.That(result[0].PayerId, Is.EqualTo(payments[0].PayerId));
            Assert.That(result[0].MentorId, Is.EqualTo(payments[0].MentorId));
            Assert.That(result[0].Amount, Is.EqualTo(payments[0].Amount));
            Assert.That(result[0].Status, Is.EqualTo(payments[0].Status));
        });
    }

    [Test]
    public void GetPaymentsByPayerId_ReturnsNotFoundException_WhenPayerIdDontExist()
    {
        paymentsRepositoryMock.Setup(repo => repo.GetPaymentsByPayerId(It.IsAny<Guid>())).ReturnsAsync((List<Payments>)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await paymentsService.GetPaymentsByPayerId(Guid.NewGuid()));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("No payments found to that payer."));
        });
    }

    [Test]
    public async Task GetPaymentsByMentorId_ReturnsPayments()
    {
        var payments = CreatePaymentsTemplate();

        paymentsRepositoryMock.Setup(repo => repo.GetPaymentsByMentorId(payments[0].MentorId)).ReturnsAsync(payments);

        var result = await paymentsService.GetPaymentsByMentorId(payments[0].MentorId);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(payments[0].Id));
            Assert.That(result[0].PayerId, Is.EqualTo(payments[0].PayerId));
            Assert.That(result[0].MentorId, Is.EqualTo(payments[0].MentorId));
            Assert.That(result[0].Amount, Is.EqualTo(payments[0].Amount));
            Assert.That(result[0].Status, Is.EqualTo(payments[0].Status));
        });
    }

    [Test]
    public void GetPaymentsByMentorId_ReturnsNotFoundException_WhenMentorIdDontExist()
    {
        paymentsRepositoryMock.Setup(repo => repo.GetPaymentsByMentorId(It.IsAny<Guid>())).ReturnsAsync((List<Payments>)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await paymentsService.GetPaymentsByMentorId(Guid.NewGuid()));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("No payments found to that mentor."));
        });
    }

    [Test]
    public async Task SendPayment_AddsPayment()
    {
        var payment = CreatePaymentsTemplate()[0];

        paymentsRepositoryMock.Setup(repo => repo.SendPayment(payment)).ReturnsAsync(payment);

        var result = await paymentsService.SendPayment(payment);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(payment.Id));
            Assert.That(result.PayerId, Is.EqualTo(payment.PayerId));
            Assert.That(result.MentorId, Is.EqualTo(payment.MentorId));
            Assert.That(result.Amount, Is.EqualTo(payment.Amount));
            Assert.That(result.Status, Is.EqualTo(payment.Status));
        });
    }

    [Test]
    public void SendPayment_ReturnsBadRequestException_WhenAmountIsNegative()
    {
        var payment = CreateInvalidPaymentTemplate();

        paymentsRepositoryMock.Setup(repo => repo.SendPayment(payment)).ReturnsAsync(payment);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await paymentsService.SendPayment(payment));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(exception.Message, Is.EqualTo("Amount must be a positive number greater than zero."));
        });
    }

    [Test]
    public async Task UpdatePaymentStatus_UpdatesPaymentStatus()
    {
        var payment = CreatePaymentsTemplate()[0];
        var updatedStatus = PaymentStatus.Completed;

        paymentsRepositoryMock.Setup(repo => repo.GetPaymentById(payment.Id)).ReturnsAsync(payment);
        paymentsRepositoryMock.Setup(repo => repo.UpdatePaymentStatus(payment.Id, updatedStatus))
                              .ReturnsAsync(new Payments()
                                {
                                    Id = payment.Id,
                                    PayerId = payment.PayerId,
                                    MentorId = payment.MentorId,
                                    Amount = payment.Amount,
                                    Status = updatedStatus
                                });

        var result = await paymentsService.UpdatePaymentStatus(payment.Id, PaymentStatus.Completed);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(payment.Id));
            Assert.That(result.PayerId, Is.EqualTo(payment.PayerId));
            Assert.That(result.MentorId, Is.EqualTo(payment.MentorId));
            Assert.That(result.Amount, Is.EqualTo(payment.Amount));
            Assert.That(result.Status, Is.EqualTo(PaymentStatus.Completed));
        });
    }

    [Test]
    public void UpdatePaymentStatus_ReturnsNotFoundException_WhenPaymentDoesNotExist()
    {
        paymentsRepositoryMock.Setup(repo => repo.GetPaymentById(It.IsAny<Guid>())).ReturnsAsync((Payments)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () =>
            await paymentsService.UpdatePaymentStatus(Guid.NewGuid(), PaymentStatus.Completed));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("Payment not found."));
        });
    }

    [Test]
    public void UpdatePaymentStatus_ReturnsBadRequestException_WhenUpdateFails()
    {
        var payment = CreatePaymentsTemplate()[0];

        paymentsRepositoryMock.Setup(repo => repo.GetPaymentById(payment.Id)).ReturnsAsync(payment);
        paymentsRepositoryMock.Setup(repo => repo.UpdatePaymentStatus(payment.Id, It.IsAny<PaymentStatus>()))
                              .ReturnsAsync((Payments)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () =>
            await paymentsService.UpdatePaymentStatus(payment.Id, PaymentStatus.Completed));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(exception.Message, Is.EqualTo("Failed to update payment status."));
        });
    }

    #region Private Methods

    private static List<Payments> CreatePaymentsTemplate()
    {
        return new List<Payments>()
        {
            new() 
            {
                Id = Guid.NewGuid(),
                PayerId = Guid.NewGuid(),
                MentorId = Guid.NewGuid(),
                Amount = 100,
                Status = PaymentStatus.Pending
            }
        };
    }

    private static Payments CreateInvalidPaymentTemplate()
    {
        return new Payments()
        {
            Id = Guid.NewGuid(),
            PayerId = Guid.NewGuid(),
            MentorId = Guid.NewGuid(),
            Amount = -100,
            Status = PaymentStatus.Pending
        };
    }

    #endregion
}
