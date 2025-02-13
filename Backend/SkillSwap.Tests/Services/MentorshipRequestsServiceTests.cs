using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;
using System.Net;

namespace SkillSwap.Tests.Services;

[TestFixture]
public class MentorshipRequestsServiceTests
{
    private Mock<IMentorshipRequestsRepository> mentorshipRequestsRepositoryMock;
    private MentorshipRequestsService mentorshipRequestsService;

    [SetUp]
    public void Setup()
    {
        mentorshipRequestsRepositoryMock = new Mock<IMentorshipRequestsRepository>();
        mentorshipRequestsService = new MentorshipRequestsService(mentorshipRequestsRepositoryMock.Object);
    }

    [Test]
    public async Task GetMentorshipRequestById_ReturnsMentorshipRequest()
    {
        var mentorshipRequest = CreateMentorshipRequestsTemplate()[0];

        mentorshipRequestsRepositoryMock.Setup(repo => repo.GetMentorshipRequestById(mentorshipRequest.Id))
                                        .ReturnsAsync(mentorshipRequest);

        var result = await mentorshipRequestsService.GetMentorshipRequestById(mentorshipRequest.Id);

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
    public void GetMentorshipRequestById_ReturnsNotFoundException_WhenMentorshipRequestIdDontExist()
    {
        mentorshipRequestsRepositoryMock.Setup(repo => repo.GetMentorshipRequestById(It.IsAny<Guid>()))
                                        .ReturnsAsync((MentorshipRequests)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await mentorshipRequestsService.GetMentorshipRequestById(Guid.NewGuid()));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("Mentorship Request not found."));
        });
    }


    [Test]
    public async Task GetMentorshipRequestsbyLearnerId_ReturnsMentorshipRequests()
    {
        var mentorshipRequests = CreateMentorshipRequestsTemplate();

        mentorshipRequestsRepositoryMock.Setup(repo => repo.GetMentorshipRequestsbyLearnerId(mentorshipRequests[0].LearnerId))
                                        .ReturnsAsync(mentorshipRequests);

        var result = await mentorshipRequestsService.GetMentorshipRequestsbyLearnerId(mentorshipRequests[0].LearnerId);

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
    public void GetMentorshipRequestsbyLearnerId_ReturnsNotFoundException_WhenLearnerIdDontExist()
    {
        mentorshipRequestsRepositoryMock.Setup(repo => repo.GetMentorshipRequestsbyLearnerId(It.IsAny<Guid>()))
                                        .ReturnsAsync((List<MentorshipRequests>)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await mentorshipRequestsService.GetMentorshipRequestsbyLearnerId(Guid.NewGuid()));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("No mentorship requests found to that learner."));
        });
    }

    [Test]
    public async Task GetMentorshipRequestsbyMentorId_ReturnsMentorshipRequests()
    {
        var mentorshipRequests = CreateMentorshipRequestsTemplate();

        mentorshipRequestsRepositoryMock.Setup(repo => repo.GetMentorshipRequestsbyMentorId(mentorshipRequests[0].MentorId))
                                        .ReturnsAsync(mentorshipRequests);

        var result = await mentorshipRequestsService.GetMentorshipRequestsbyMentorId(mentorshipRequests[0].MentorId);

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
    public void GetMentorshipRequestsbyMentorId_ReturnsNotFoundException_WhenMentorIdDontExist()
    {
        mentorshipRequestsRepositoryMock.Setup(repo => repo.GetMentorshipRequestsbyMentorId(It.IsAny<Guid>()))
                                        .ReturnsAsync((List<MentorshipRequests>)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await mentorshipRequestsService.GetMentorshipRequestsbyMentorId(Guid.NewGuid()));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("No mentorship requests found to that mentor."));
        });
    }

    [Test]
    public async Task CreateMentorshipRequest_CreatesSuccessfully()
    {
        var mentorshipRequest = CreateMentorshipRequestsTemplate()[0];

        mentorshipRequestsRepositoryMock.Setup(repo => repo.CreateMentorshipRequest(mentorshipRequest))
                                        .ReturnsAsync(mentorshipRequest);

        var result = await mentorshipRequestsService.CreateMentorshipRequest(mentorshipRequest);

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
    public async Task UpdateMentorshipRequest_UpdatesSuccessfully()
    {
        var mentorshipRequest = CreateMentorshipRequestsTemplate()[0];
        var updateMentorshipRequest = UpdateMentorshipRequestsTemplate(mentorshipRequest.Id, mentorshipRequest.MentorId, 
                                                                        mentorshipRequest.LearnerId, mentorshipRequest.SkillId);

        mentorshipRequestsRepositoryMock.Setup(repo => repo.CreateMentorshipRequest(mentorshipRequest))
                                        .ReturnsAsync(mentorshipRequest);
        mentorshipRequestsRepositoryMock.Setup(repo => repo.UpdateMentorshipRequest(updateMentorshipRequest))
                                        .Returns(Task.CompletedTask);
        mentorshipRequestsRepositoryMock.Setup(repo => repo.GetMentorshipRequestById(mentorshipRequest.Id))
                                        .ReturnsAsync(mentorshipRequest);

        await mentorshipRequestsService.UpdateMentorshipRequest(mentorshipRequest.Id, mentorshipRequest);
        var result = await mentorshipRequestsService.GetMentorshipRequestById(mentorshipRequest.Id);

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
    public async Task DeleteMentorshipRequest_DeletesSuccessfully()
    {
        var mentorshipRequest = CreateMentorshipRequestsTemplate()[0];

        mentorshipRequestsRepositoryMock.Setup(repo => repo.CreateMentorshipRequest(mentorshipRequest))
                                        .ReturnsAsync(mentorshipRequest);
        mentorshipRequestsRepositoryMock.Setup(repo => repo.DeleteMentorshipRequest(mentorshipRequest.Id))
                                        .Returns(Task.CompletedTask);
        mentorshipRequestsRepositoryMock.Setup(repo => repo.GetMentorshipRequestById(mentorshipRequest.Id))
                                        .ReturnsAsync((MentorshipRequests)null);

        await mentorshipRequestsService.CreateMentorshipRequest(mentorshipRequest);
        await mentorshipRequestsService.DeleteMentorshipRequest(mentorshipRequest.Id);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await mentorshipRequestsService.GetMentorshipRequestById(mentorshipRequest.Id));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("Mentorship Request not found."));
        });
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
                ScheduledAt = DateTime.Now
            }
        };
    }

    private static MentorshipRequests UpdateMentorshipRequestsTemplate(Guid id, Guid mentorId, Guid learnerId, Guid skillId)
    {
        return new MentorshipRequests()
        {
            Id = id,
            MentorId = mentorId,
            LearnerId = learnerId,
            SkillId = skillId,
            Status = MentorshipRequestsStatus.Accepted,
            ScheduledAt = DateTime.Now
        };
    }

    #endregion
}
