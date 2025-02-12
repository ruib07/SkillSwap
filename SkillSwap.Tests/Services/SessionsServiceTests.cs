using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;
using System.Net;

namespace SkillSwap.Tests.Services;

[TestFixture]
public class SessionsServiceTests
{
    private Mock<ISessionsRepository> sessionsRepositoryMock;
    private SessionsService sessionsService;

    [SetUp]
    public void Setup()
    {
        sessionsRepositoryMock = new Mock<ISessionsRepository>();
        sessionsService = new SessionsService(sessionsRepositoryMock.Object);
    }

    [Test]
    public async Task GetSessionById_ReturnsSession()
    {
        var session = CreateSessionTemplate()[0];

        sessionsRepositoryMock.Setup(repo => repo.GetSessionById(session.Id)).ReturnsAsync(session);

        var result = await sessionsService.GetSessionById(session.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(session.Id));
            Assert.That(result.MentorshipRequestId, Is.EqualTo(session.MentorshipRequestId));
            Assert.That(result.SessionTime, Is.EqualTo(session.SessionTime));
            Assert.That(result.Duration, Is.EqualTo(session.Duration));
            Assert.That(result.VideoLink, Is.EqualTo(session.VideoLink));
        });
    }

    [Test]
    public void GetSessionById_ReturnsNotFoundException_WhenSessionIdDontExist()
    {
        sessionsRepositoryMock.Setup(repo => repo.GetSessionById(It.IsAny<Guid>())).ReturnsAsync((Sessions)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await sessionsService.GetSessionById(Guid.NewGuid()));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("Session not found."));
        });
    }

    [Test]
    public async Task GetSessionsByMentorshipRequestId_ReturnsSessions()
    {
        var sessions = CreateSessionTemplate();

        sessionsRepositoryMock.Setup(repo => repo.GetSessionsByMentorshipRequestId(sessions[0].MentorshipRequestId)).ReturnsAsync(sessions);

        var result = await sessionsService.GetSessionsByMentorshipRequestId(sessions[0].MentorshipRequestId);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(sessions.Count));
            Assert.That(result[0].Id, Is.EqualTo(sessions[0].Id));
            Assert.That(result[0].MentorshipRequestId, Is.EqualTo(sessions[0].MentorshipRequestId));
            Assert.That(result[0].SessionTime, Is.EqualTo(sessions[0].SessionTime));
            Assert.That(result[0].Duration, Is.EqualTo(sessions[0].Duration));
            Assert.That(result[0].VideoLink, Is.EqualTo(sessions[0].VideoLink));
        });
    }

    [Test]
    public void GetSessionsByMentorshipRequestId_ReturnsNotFoundException_WhenMentorshipRequestIdDontExist()
    {
        sessionsRepositoryMock.Setup(repo => repo.GetSessionsByMentorshipRequestId(It.IsAny<Guid>())).ReturnsAsync((List<Sessions>)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await sessionsService.GetSessionsByMentorshipRequestId(Guid.NewGuid()));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("No sessions found to that mentorship request."));
        });
    }

    [Test]
    public async Task CreateSession_CreatesSuccessfully()
    {
        var session = CreateSessionTemplate()[0];

        sessionsRepositoryMock.Setup(repo => repo.CreateSession(session)).ReturnsAsync(session);

        var result = await sessionsService.CreateSession(session);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(session.Id));
            Assert.That(result.MentorshipRequestId, Is.EqualTo(session.MentorshipRequestId));
            Assert.That(result.SessionTime, Is.EqualTo(session.SessionTime));
            Assert.That(result.Duration, Is.EqualTo(session.Duration));
            Assert.That(result.VideoLink, Is.EqualTo(session.VideoLink));
        });
    }

    [Test]
    public async Task UpdateSession_UpdatesSuccessfully()
    {
        var session = CreateSessionTemplate()[0];

        var updateSession = UpdateSessionTemplate(session.Id, session.MentorshipRequestId);

        sessionsRepositoryMock.Setup(repo => repo.CreateSession(session)).ReturnsAsync(session);
        sessionsRepositoryMock.Setup(repo => repo.UpdateSession(updateSession)).Returns(Task.CompletedTask);
        sessionsRepositoryMock.Setup(repo => repo.GetSessionById(session.Id)).ReturnsAsync(session);

        await sessionsService.UpdateSession(session.Id, updateSession);
        var result = await sessionsService.GetSessionById(session.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(session.Id));
            Assert.That(result.MentorshipRequestId, Is.EqualTo(session.MentorshipRequestId));
            Assert.That(result.SessionTime, Is.EqualTo(session.SessionTime));
            Assert.That(result.Duration, Is.EqualTo(session.Duration));
            Assert.That(result.VideoLink, Is.EqualTo(session.VideoLink));
        });
    }

    [Test]
    public async Task DeleteSession_DeletesSuccessfully()
    {
        var session = CreateSessionTemplate()[0];

        sessionsRepositoryMock.Setup(repo => repo.CreateSession(session)).ReturnsAsync(session);
        sessionsRepositoryMock.Setup(repo => repo.DeleteSession(session.Id)).Returns(Task.CompletedTask);
        sessionsRepositoryMock.Setup(repo => repo.GetSessionById(session.Id)).ReturnsAsync((Sessions)null);

        await sessionsService.CreateSession(session);
        await sessionsService.DeleteSession(session.Id);
        var exception = Assert.ThrowsAsync<CustomException>(async () => await sessionsService.GetSessionById(session.Id));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("Session not found."));
        });
    }

    #region Private Methods

    private static List<Sessions> CreateSessionTemplate()
    {
        return new List<Sessions>()
        {
            new() 
            {
                Id = Guid.NewGuid(),
                MentorshipRequestId = Guid.NewGuid(),
                SessionTime = DateTime.Now,
                Duration = 60,
                VideoLink = "https://www.youtube.com/watch?v=123456"
            }
        };
    }

    private static Sessions UpdateSessionTemplate(Guid id, Guid mentorshipRequestId)
    {
        return new Sessions()
        {
            Id = id,
            MentorshipRequestId = mentorshipRequestId,
            SessionTime = DateTime.Now,
            Duration = 30,
            VideoLink = "https://www.youtube.com/watch?v=123456"
        };
    }

    #endregion
}
