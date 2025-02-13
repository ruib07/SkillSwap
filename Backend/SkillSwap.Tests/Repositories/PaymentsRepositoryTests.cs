using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Repositories;

namespace SkillSwap.Tests.Repositories;

[TestFixture]
public class PaymentsRepositoryTests
{
    private PaymentsRepository paymentsRepository;
    private SkillSwapDbContext context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SkillSwapDbContext>()
                     .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                     .Options;

        context = new SkillSwapDbContext(options);
        paymentsRepository = new PaymentsRepository(context);
    }

    [TearDown]
    public void TearDown()
    {
        context.Dispose();
    }

    [Test]
    public async Task GetPaymentById_ReturnsPayment()
    {
        var payment = CreatePaymentsTemplate()[0];
        await paymentsRepository.SendPayment(payment);

        var result = await paymentsRepository.GetPaymentById(payment.Id);

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
    public async Task GetPaymentsByMentorId_ReturnsPayment()
    {
        var payments = CreatePaymentsTemplate();
        context.Payments.AddRange(payments);
        await context.SaveChangesAsync();

        var result = await paymentsRepository.GetPaymentsByPayerId(payments[0].PayerId);

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
    public async Task GetPaymentsByPayerId_ReturnsPayment()
    {
        var payments = CreatePaymentsTemplate();
        context.Payments.AddRange(payments);
        await context.SaveChangesAsync();

        var result = await paymentsRepository.GetPaymentsByMentorId(payments[1].MentorId);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(payments[1].Id));
            Assert.That(result[0].PayerId, Is.EqualTo(payments[1].PayerId));
            Assert.That(result[0].MentorId, Is.EqualTo(payments[1].MentorId));
            Assert.That(result[0].Amount, Is.EqualTo(payments[1].Amount));
            Assert.That(result[0].Status, Is.EqualTo(payments[1].Status));
        });
    }

    [Test]
    public async Task SendPayment_AddsPayment()
    {
        var newPayment = CreatePaymentsTemplate()[1];

        var result = await paymentsRepository.SendPayment(newPayment);
        var addedPayment = await paymentsRepository.GetPaymentById(newPayment.Id);

        Assert.That(addedPayment, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(addedPayment.Id, Is.EqualTo(newPayment.Id));
            Assert.That(addedPayment.PayerId, Is.EqualTo(newPayment.PayerId));
            Assert.That(addedPayment.MentorId, Is.EqualTo(newPayment.MentorId));
            Assert.That(addedPayment.Amount, Is.EqualTo(newPayment.Amount));
            Assert.That(addedPayment.Status, Is.EqualTo(newPayment.Status));
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
                Amount = 149.99m,
                Status = PaymentStatus.Pending,
            },
            new()
            {
                Id = Guid.NewGuid(),
                PayerId = Guid.NewGuid(),
                MentorId = Guid.NewGuid(),
                Amount = 199.99m,
                Status = PaymentStatus.Completed,
            }
        };
    }

    #endregion
}
