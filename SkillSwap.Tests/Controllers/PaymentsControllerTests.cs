using Microsoft.AspNetCore.Mvc;
using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Controllers;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;
using static SkillSwap.Server.Models.Responses;

namespace SkillSwap.Tests.Controllers;

[TestFixture]
public class PaymentsControllerTests
{
    private Mock<IPaymentsRepository> paymentsRepositoryMock;
    private PaymentsService paymentsService;
    private PaymentsController paymentsController;

    [SetUp]
    public void Setup()
    {
        paymentsRepositoryMock = new Mock<IPaymentsRepository>();
        paymentsService = new PaymentsService(paymentsRepositoryMock.Object);
        paymentsController = new PaymentsController(paymentsService);
    }

    [Test]
    public async Task GetPaymentById_ReturnsOkResult_WithPayment()
    {
        var payment = CreatePaymentTemplate()[0];

        paymentsRepositoryMock.Setup(repo => repo.GetPaymentById(payment.Id)).ReturnsAsync(payment);

        var result = await paymentsController.GetPaymentById(payment.Id);
        var okResult = result as OkObjectResult;
        var response = okResult.Value as Payments;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response.PayerId, Is.EqualTo(payment.PayerId));
            Assert.That(response.MentorId, Is.EqualTo(payment.MentorId));
            Assert.That(response.Amount, Is.EqualTo(payment.Amount));
            Assert.That(response.Status, Is.EqualTo(payment.Status));
        });
    }

    [Test]
    public async Task GetPaymentsByPayerId_ReturnsOkResult_WithPayment()
    {
        var payment = CreatePaymentTemplate();

        paymentsRepositoryMock.Setup(repo => repo.GetPaymentsByPayerId(payment[0].PayerId)).ReturnsAsync(payment);

        var result = await paymentsController.GetPaymentsByPayerId(payment[0].PayerId);
        var okResult = result.Result as OkObjectResult;
        var response = okResult.Value as List<Payments>;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response[0].PayerId, Is.EqualTo(payment[0].PayerId));
            Assert.That(response[0].MentorId, Is.EqualTo(payment[0].MentorId));
            Assert.That(response[0].Amount, Is.EqualTo(payment[0].Amount));
            Assert.That(response[0].Status, Is.EqualTo(payment[0].Status));
        });
    }

    [Test]
    public async Task GetPaymentsByMentorId_ReturnsOkResult_WithPayment()
    {
        var payment = CreatePaymentTemplate();

        paymentsRepositoryMock.Setup(repo => repo.GetPaymentsByMentorId(payment[0].MentorId)).ReturnsAsync(payment);

        var result = await paymentsController.GetPaymentsByMentorId(payment[0].MentorId);
        var okResult = result.Result as OkObjectResult;
        var response = okResult.Value as List<Payments>;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response[0].PayerId, Is.EqualTo(payment[0].PayerId));
            Assert.That(response[0].MentorId, Is.EqualTo(payment[0].MentorId));
            Assert.That(response[0].Amount, Is.EqualTo(payment[0].Amount));
            Assert.That(response[0].Status, Is.EqualTo(payment[0].Status));
        });
    }

    [Test]
    public async Task SendPayment_ReturnsCreatedResult_WithValidPayment()
    {
        var newPayment = CreatePaymentTemplate()[0];

        paymentsRepositoryMock.Setup(repo => repo.SendPayment(It.IsAny<Payments>())).ReturnsAsync(newPayment);

        var result = await paymentsController.SendPayment(newPayment);
        var createdResult = result.Result as ObjectResult;
        var response = createdResult.Value as CreationResponse;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
            Assert.That(response.Message, Is.EqualTo("Payment created successfully."));
            Assert.That(response.Id, Is.EqualTo(newPayment.Id));
        });
    }

    #region Private Methods

    private static List<Payments> CreatePaymentTemplate()
    {
        return new List<Payments>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                PayerId = Guid.NewGuid(),
                MentorId = Guid.NewGuid(),
                Amount = 49.99m,
                Status = PaymentStatus.Completed
            }
        };
    }

    #endregion
}
