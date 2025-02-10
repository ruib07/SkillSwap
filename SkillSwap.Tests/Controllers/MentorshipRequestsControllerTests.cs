using Microsoft.AspNetCore.Mvc;
using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Controllers;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;
using static SkillSwap.Server.Models.Responses;

namespace SkillSwap.Tests.Controllers;

[TestFixture]
public class MentorshipRequestsControllerTests
{
    private Mock<IMentorshipRequestsRepository> mentorshipRequestsRepositoryMock;
    private MentorshipRequestsService mentorshipRequestsService;
    private MentorshipRequestsController mentorshipRequestsController;

    [SetUp]
    public void Setup()
    {
        mentorshipRequestsRepositoryMock = new Mock<IMentorshipRequestsRepository>();
        mentorshipRequestsService = new MentorshipRequestsService(mentorshipRequestsRepositoryMock.Object);
        mentorshipRequestsController = new MentorshipRequestsController(mentorshipRequestsService);
    }

    [Test]
    public async Task GetMentorshipRequestById_ReturnsOkResult_WithMentorshipRequest()
    {
        var mentorshipRequest = CreateMentorshipRequestsTemplate()[0];

        mentorshipRequestsRepositoryMock.Setup(repo => repo.GetMentorshipRequestById(mentorshipRequest.Id))
                                        .ReturnsAsync(mentorshipRequest);

        var result = await mentorshipRequestsController.GetMentorshipRequestById(mentorshipRequest.Id);
        var okResult = result as OkObjectResult;
        var response = okResult.Value as MentorshipRequests;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response.MentorId, Is.EqualTo(mentorshipRequest.MentorId));
            Assert.That(response.LearnerId, Is.EqualTo(mentorshipRequest.LearnerId));
            Assert.That(response.SkillId, Is.EqualTo(mentorshipRequest.SkillId));
            Assert.That(response.Status, Is.EqualTo(mentorshipRequest.Status));
            Assert.That(response.ScheduledAt, Is.EqualTo(mentorshipRequest.ScheduledAt));
        });
    }

    [Test]
    public async Task GetMentorshipRequestsbyLearnerId_ReturnsOkResult_WithMentorshipRequest()
    {
        var mentorshipRequest = CreateMentorshipRequestsTemplate();

        mentorshipRequestsRepositoryMock.Setup(repo => repo.GetMentorshipRequestsbyLearnerId(mentorshipRequest[0].LearnerId))
                                        .ReturnsAsync(mentorshipRequest);

        var result = await mentorshipRequestsController.GetMentorshipRequestsbyLearnerId(mentorshipRequest[0].LearnerId);
        var okResult = result.Result as OkObjectResult;
        var response = okResult.Value as List<MentorshipRequests>;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response[0].MentorId, Is.EqualTo(mentorshipRequest[0].MentorId));
            Assert.That(response[0].LearnerId, Is.EqualTo(mentorshipRequest[0].LearnerId));
            Assert.That(response[0].SkillId, Is.EqualTo(mentorshipRequest[0].SkillId));
            Assert.That(response[0].Status, Is.EqualTo(mentorshipRequest[0].Status));
            Assert.That(response[0].ScheduledAt, Is.EqualTo(mentorshipRequest[0].ScheduledAt));
        });
    }

    [Test]
    public async Task GetMentorshipRequestsbyMentorId_ReturnsOkResult_WithMentorshipRequest()
    {
        var mentorshipRequest = CreateMentorshipRequestsTemplate();

        mentorshipRequestsRepositoryMock.Setup(repo => repo.GetMentorshipRequestsbyMentorId(mentorshipRequest[0].MentorId))
                                        .ReturnsAsync(mentorshipRequest);

        var result = await mentorshipRequestsController.GetMentorshipRequestsbyMentorId(mentorshipRequest[0].MentorId);
        var okResult = result.Result as OkObjectResult;
        var response = okResult.Value as List<MentorshipRequests>;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response[0].MentorId, Is.EqualTo(mentorshipRequest[0].MentorId));
            Assert.That(response[0].LearnerId, Is.EqualTo(mentorshipRequest[0].LearnerId));
            Assert.That(response[0].SkillId, Is.EqualTo(mentorshipRequest[0].SkillId));
            Assert.That(response[0].Status, Is.EqualTo(mentorshipRequest[0].Status));
            Assert.That(response[0].ScheduledAt, Is.EqualTo(mentorshipRequest[0].ScheduledAt));
        });
    }

    [Test]
    public async Task CreateMentorshipRequest_ReturnsCreatedResult_WithValidMentorshipRequest()
    {
        var newMentorshipRequest = CreateMentorshipRequestsTemplate()[0];

        mentorshipRequestsRepositoryMock.Setup(repo => repo.CreateMentorshipRequest(It.IsAny<MentorshipRequests>()))
                                        .ReturnsAsync(newMentorshipRequest);

        var result = await mentorshipRequestsController.CreateMentorshipRequest(newMentorshipRequest);
        var createdResult = result.Result as ObjectResult;
        var response = createdResult.Value as CreationResponse;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
            Assert.That(response.Message, Is.EqualTo("Mentorship request created successfully."));
            Assert.That(response.Id, Is.EqualTo(newMentorshipRequest.Id));
        });
    }

    [Test]
    public async Task UpdateMentorshipRequest_ReturnsOkResult_WithUpdatedMentorshipRequest()
    {
        var mentorshipRequestToUpdate = CreateMentorshipRequestsTemplate()[0];
        var updatedMentorshipRequest = UpdateMentorshipRequestTemplate();

        mentorshipRequestsRepositoryMock.Setup(repo => repo.GetMentorshipRequestById(mentorshipRequestToUpdate.Id))
                                        .ReturnsAsync(mentorshipRequestToUpdate);
        mentorshipRequestsRepositoryMock.Setup(repo => repo.UpdateMentorshipRequest(It.IsAny<MentorshipRequests>()))
                                        .Returns(Task.CompletedTask);

        var result = await mentorshipRequestsController.UpdateMentorshipRequest(mentorshipRequestToUpdate.Id, updatedMentorshipRequest);
        var okResult = result as OkObjectResult;

        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo("Mentorship request updated successfully."));
        });
    }

    [Test]
    public async Task DeleteMentorshipRequest_ReturnsNoContent_WhenMentorshipRequestIsDeleted()
    {
        var mentorshipRequest = CreateMentorshipRequestsTemplate()[0];

        mentorshipRequestsRepositoryMock.Setup(repo => repo.GetMentorshipRequestById(mentorshipRequest.Id)).ReturnsAsync(mentorshipRequest);
        mentorshipRequestsRepositoryMock.Setup(repo => repo.DeleteMentorshipRequest(mentorshipRequest.Id)).Returns(Task.CompletedTask);

        var result = await mentorshipRequestsController.DeleteMentorshipRequest(mentorshipRequest.Id);
        var noContentResult = result as NoContentResult;

        Assert.That(noContentResult.StatusCode, Is.EqualTo(204));
    }

    #region Private methods

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
                ScheduledAt = DateTime.UtcNow,
            }
        };
    }

    private static MentorshipRequests UpdateMentorshipRequestTemplate()
    {
        return new MentorshipRequests()
        {
            MentorId = Guid.NewGuid(),
            LearnerId = Guid.NewGuid(),
            SkillId = Guid.NewGuid(),
            Status = MentorshipRequestsStatus.Completed,
            ScheduledAt = DateTime.UtcNow.AddDays(3),
        };
    }

    #endregion
}
