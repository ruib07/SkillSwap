using Microsoft.AspNetCore.Mvc;
using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Controllers;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;
using static SkillSwap.Server.Models.Responses;

namespace SkillSwap.Tests.Controllers;

[TestFixture]
public class SessionsControllerTests
{
    private Mock<ISessionsRepository> sessionsRepositoryMock;
    private SessionsService sessionsService;
    private SessionsController sessionsController;

    [SetUp]
    public void Setup()
    {
        sessionsRepositoryMock = new Mock<ISessionsRepository>();
        sessionsService = new SessionsService(sessionsRepositoryMock.Object);
        sessionsController = new SessionsController(sessionsService);
    }

    [Test]
    public async Task GetSessionById_ReturnsOkResult_WithSession()
    {
        var session = CreateSessionTemplate()[0];

        sessionsRepositoryMock.Setup(repo => repo.GetSessionById(session.Id)).ReturnsAsync(session);

        var result = await sessionsController.GetSessionById(session.Id);
        var okResult = result as OkObjectResult;
        var response = okResult.Value as Sessions;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response.MentorshipRequestId, Is.EqualTo(session.MentorshipRequestId));
            Assert.That(response.SessionTime, Is.EqualTo(session.SessionTime));
            Assert.That(response.Duration, Is.EqualTo(session.Duration));
            Assert.That(response.VideoLink, Is.EqualTo(session.VideoLink));
        });
    }

    [Test]
    public async Task GetSessionsByMentorshipRequestId_ReturnsOkResult_WithSession()
    {
        var session = CreateSessionTemplate();

        sessionsRepositoryMock.Setup(repo => repo.GetSessionsByMentorshipRequestId(session[0].MentorshipRequestId))
                              .ReturnsAsync(session);

        var result = await sessionsController.GetSessionsByMentorshipRequestId(session[0].MentorshipRequestId);
        var okResult = result.Result as OkObjectResult;
        var response = okResult.Value as List<Sessions>;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response[0].MentorshipRequestId, Is.EqualTo(session[0].MentorshipRequestId));
            Assert.That(response[0].SessionTime, Is.EqualTo(session[0].SessionTime));
            Assert.That(response[0].Duration, Is.EqualTo(session[0].Duration));
            Assert.That(response[0].VideoLink, Is.EqualTo(session[0].VideoLink));
        });
    }

    [Test]
    public async Task CreateSession_ReturnsCreatedResult_WithValidSession()
    {
        var newSession = CreateSessionTemplate()[0];

        sessionsRepositoryMock.Setup(repo => repo.CreateSession(It.IsAny<Sessions>())).ReturnsAsync(newSession);

        var result = await sessionsController.CreateSession(newSession);
        var createdResult = result.Result as ObjectResult;
        var response = createdResult.Value as CreationResponse;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
            Assert.That(response.Message, Is.EqualTo("Session created successfully."));
            Assert.That(response.Id, Is.EqualTo(newSession.Id));
        });
    }

    [Test]
    public async Task UpdateSession_ReturnsOkResult_WithUpdatedSession()
    {
        var sessionToUpdate = CreateSessionTemplate()[0];
        var updatedSession = UpdateSessionTemplate();

        sessionsRepositoryMock.Setup(repo => repo.GetSessionById(sessionToUpdate.Id)).ReturnsAsync(sessionToUpdate);
        sessionsRepositoryMock.Setup(repo => repo.UpdateSession(It.IsAny<Sessions>())).Returns(Task.CompletedTask);

        var result = await sessionsController.UpdateSession(sessionToUpdate.Id, updatedSession);
        var okResult = result as OkObjectResult;

        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo("Session updated successfully."));
        });
    }

    [Test]
    public async Task DeleteSession_ReturnsNoContent_WhenSessionIsDeleted()
    {
        var session = CreateSessionTemplate()[0];

        sessionsRepositoryMock.Setup(repo => repo.GetSessionById(session.Id)).ReturnsAsync(session);
        sessionsRepositoryMock.Setup(repo => repo.DeleteSession(session.Id)).Returns(Task.CompletedTask);

        var result = await sessionsController.DeleteSession(session.Id);
        var noContentResult = result as NoContentResult;

        Assert.That(noContentResult.StatusCode, Is.EqualTo(204));
    }

    #region Private Methods

    private static List<Sessions> CreateSessionTemplate()
    {
        return new List<Sessions>()
        {
            new() {
                Id = Guid.NewGuid(),
                MentorshipRequestId = Guid.NewGuid(),
                SessionTime = DateTime.Now,
                Duration = 30,
                VideoLink = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
                Amount = 139.99m
            }
        };
    }

    private static Sessions UpdateSessionTemplate()
    {
        return new Sessions()
        {
            MentorshipRequestId = Guid.NewGuid(),
            SessionTime = DateTime.UtcNow.AddDays(3),
            Duration = 40,
            VideoLink = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            Amount = 199.99m
        };
    }

    #endregion
}
