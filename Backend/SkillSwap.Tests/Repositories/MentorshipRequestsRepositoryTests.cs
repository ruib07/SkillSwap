using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Repositories;

namespace SkillSwap.Tests.Repositories;

[TestFixture]
public class MentorshipRequestsRepositoryTests
{
    private MentorshipRequestsRepository mentorshipRequestsRepository;
    private SkillSwapDbContext context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SkillSwapDbContext>()
                     .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                     .Options;

        context = new SkillSwapDbContext(options);
        mentorshipRequestsRepository = new MentorshipRequestsRepository(context);
    }

    [TearDown]
    public void TearDown()
    {
        context.Dispose();
    }

    [Test]
    public async Task GetMentorshipRequestById_ReturnsMentorshipRequest()
    {
        var mentorshipRequest = CreateMentorshipRequestsTemplate()[0];
        await mentorshipRequestsRepository.CreateMentorshipRequest(mentorshipRequest);

        var result = await mentorshipRequestsRepository.GetMentorshipRequestById(mentorshipRequest.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(mentorshipRequest.Id));
            Assert.That(result.MentorId, Is.EqualTo(mentorshipRequest.MentorId));
            Assert.That(result.LearnerId, Is.EqualTo(mentorshipRequest.LearnerId));
            Assert.That(result.SkillId, Is.EqualTo(mentorshipRequest.SkillId));
            Assert.That(result.Status, Is.EqualTo(mentorshipRequest.Status));
            Assert.That(result.ScheduledAt, Is.EqualTo(mentorshipRequest.ScheduledAt));
        });
    }

    [Test]
    public async Task GetMentorshipRequestsbyLearnerId_ReturnsMentorshipRequest()
    {
        var mentorshipRequests = CreateMentorshipRequestsTemplate();
        context.MentorshipRequests.AddRange(mentorshipRequests);
        await context.SaveChangesAsync();

        var result = await mentorshipRequestsRepository.GetMentorshipRequestsbyLearnerId(mentorshipRequests[0].LearnerId);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(mentorshipRequests[0].Id));
            Assert.That(result[0].MentorId, Is.EqualTo(mentorshipRequests[0].MentorId));
            Assert.That(result[0].LearnerId, Is.EqualTo(mentorshipRequests[0].LearnerId));
            Assert.That(result[0].SkillId, Is.EqualTo(mentorshipRequests[0].SkillId));
            Assert.That(result[0].Status, Is.EqualTo(mentorshipRequests[0].Status));
            Assert.That(result[0].ScheduledAt, Is.EqualTo(mentorshipRequests[0].ScheduledAt));
        });
    }

    [Test]
    public async Task GetMentorshipRequestsbyMentorId_ReturnsMentorshipRequest()
    {
        var mentorshipRequests = CreateMentorshipRequestsTemplate();
        context.MentorshipRequests.AddRange(mentorshipRequests);
        await context.SaveChangesAsync();

        var result = await mentorshipRequestsRepository.GetMentorshipRequestsbyMentorId(mentorshipRequests[0].MentorId);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(mentorshipRequests[0].Id));
            Assert.That(result[0].MentorId, Is.EqualTo(mentorshipRequests[0].MentorId));
            Assert.That(result[0].LearnerId, Is.EqualTo(mentorshipRequests[0].LearnerId));
            Assert.That(result[0].SkillId, Is.EqualTo(mentorshipRequests[0].SkillId));
            Assert.That(result[0].Status, Is.EqualTo(mentorshipRequests[0].Status));
            Assert.That(result[0].ScheduledAt, Is.EqualTo(mentorshipRequests[0].ScheduledAt));
        });
    }

    [Test]
    public async Task CreateMentorshipRequest_AddsMentorshipRequest()
    {
        var newMentorshipRequest = CreateMentorshipRequestsTemplate()[0];

        var result = await mentorshipRequestsRepository.CreateMentorshipRequest(newMentorshipRequest);
        var addedMentorshipRequest = await mentorshipRequestsRepository.GetMentorshipRequestById(newMentorshipRequest.Id);

        Assert.That(addedMentorshipRequest, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(addedMentorshipRequest.Id, Is.EqualTo(newMentorshipRequest.Id));
            Assert.That(addedMentorshipRequest.MentorId, Is.EqualTo(newMentorshipRequest.MentorId));
            Assert.That(addedMentorshipRequest.LearnerId, Is.EqualTo(newMentorshipRequest.LearnerId));
            Assert.That(addedMentorshipRequest.SkillId, Is.EqualTo(newMentorshipRequest.SkillId));
            Assert.That(addedMentorshipRequest.Status, Is.EqualTo(newMentorshipRequest.Status));
            Assert.That(addedMentorshipRequest.ScheduledAt, Is.EqualTo(newMentorshipRequest.ScheduledAt));
        });
    }

    [Test]
    public async Task UpdateMentorshipRequest_UpdatesSuccessfully()
    {
        var existingMentorshipRequest = CreateMentorshipRequestsTemplate()[0];
        await mentorshipRequestsRepository.CreateMentorshipRequest(existingMentorshipRequest);

        context.Entry(existingMentorshipRequest).State = EntityState.Detached;

        var updatedMentorshipRequest = UpdateMentorshipRequestTemplate(id: existingMentorshipRequest.Id);

        await mentorshipRequestsRepository.UpdateMentorshipRequest(updatedMentorshipRequest);
        var retrievedUpdatedMentorshipRequest = await mentorshipRequestsRepository.GetMentorshipRequestById(existingMentorshipRequest.Id);

        Assert.That(retrievedUpdatedMentorshipRequest, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(retrievedUpdatedMentorshipRequest.Id, Is.EqualTo(updatedMentorshipRequest.Id));
            Assert.That(retrievedUpdatedMentorshipRequest.MentorId, Is.EqualTo(updatedMentorshipRequest.MentorId));
            Assert.That(retrievedUpdatedMentorshipRequest.LearnerId, Is.EqualTo(updatedMentorshipRequest.LearnerId));
            Assert.That(retrievedUpdatedMentorshipRequest.SkillId, Is.EqualTo(updatedMentorshipRequest.SkillId));
            Assert.That(retrievedUpdatedMentorshipRequest.Status, Is.EqualTo(updatedMentorshipRequest.Status));
            Assert.That(retrievedUpdatedMentorshipRequest.ScheduledAt, Is.EqualTo(updatedMentorshipRequest.ScheduledAt));
        });
    }

    [Test]
    public async Task DeleteMentorshipRequest_DeletesSuccessfully()
    {
        var existingMentorshipRequest = CreateMentorshipRequestsTemplate()[1];

        await mentorshipRequestsRepository.CreateMentorshipRequest(existingMentorshipRequest);
        await mentorshipRequestsRepository.DeleteMentorshipRequest(existingMentorshipRequest.Id);
        var retrievedNullMentorshipRequest = await mentorshipRequestsRepository.GetMentorshipRequestById(existingMentorshipRequest.Id);

        Assert.That(retrievedNullMentorshipRequest, Is.Null);
    }

    #region Private Methods

    private static List<MentorshipRequests> CreateMentorshipRequestsTemplate()
    {
        return new List<MentorshipRequests>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                MentorId = Guid.NewGuid(),
                LearnerId = Guid.NewGuid(),
                SkillId = Guid.NewGuid(),
                Status = MentorshipRequestsStatus.Pending,
                ScheduledAt = DateTime.Now,
            },
            new()
            {
                Id = Guid.NewGuid(),
                MentorId = Guid.NewGuid(),
                LearnerId = Guid.NewGuid(),
                SkillId = Guid.NewGuid(),
                Status = MentorshipRequestsStatus.Completed,
                ScheduledAt = DateTime.Now,
            }
        };
    }

    private static MentorshipRequests UpdateMentorshipRequestTemplate(Guid id)
    {
        return new MentorshipRequests()
        {
            Id = id,
            MentorId = Guid.NewGuid(),
            LearnerId = Guid.NewGuid(),
            SkillId = Guid.NewGuid(),
            Status = MentorshipRequestsStatus.Accepted,
            ScheduledAt = DateTime.Now,
        };
    }

    #endregion
}
