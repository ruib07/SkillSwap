using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Repositories;

namespace SkillSwap.Tests.Repositories;

[TestFixture]
public class SessionsRepositoryTests
{
    private SessionsRepository sessionsRepository;
    private SkillSwapDbContext context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SkillSwapDbContext>()
                     .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                     .Options;

        context = new SkillSwapDbContext(options);
        sessionsRepository = new SessionsRepository(context);
    }

    [TearDown]
    public void TearDown()
    {
        context.Dispose();
    }

    [Test]
    public async Task GetSessionById_ReturnsSession()
    {
        var session = CreateSessionsTemplate()[0];
        await sessionsRepository.CreateSession(session);

        var result = await sessionsRepository.GetSessionById(session.Id);

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
    public async Task GetSessionsByMentorshipRequestId_ReturnsSession()
    {
        var sessions = CreateSessionsTemplate();
        context.Sessions.AddRange(sessions);
        await context.SaveChangesAsync();

        var result = await sessionsRepository.GetSessionsByMentorshipRequestId(sessions[0].MentorshipRequestId);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(sessions[0].Id));
            Assert.That(result[0].MentorshipRequestId, Is.EqualTo(sessions[0].MentorshipRequestId));
            Assert.That(result[0].SessionTime, Is.EqualTo(sessions[0].SessionTime));
            Assert.That(result[0].Duration, Is.EqualTo(sessions[0].Duration));
            Assert.That(result[0].VideoLink, Is.EqualTo(sessions[0].VideoLink));
        });
    }

    [Test]
    public async Task CreateSession_AddsSession()
    {
        var newSession = CreateSessionsTemplate()[0];

        var result = await sessionsRepository.CreateSession(newSession);
        var addedSession = await sessionsRepository.GetSessionById(newSession.Id);

        Assert.That(addedSession, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(addedSession.Id, Is.EqualTo(newSession.Id));
            Assert.That(addedSession.MentorshipRequestId, Is.EqualTo(newSession.MentorshipRequestId));
            Assert.That(addedSession.SessionTime, Is.EqualTo(newSession.SessionTime));
            Assert.That(addedSession.Duration, Is.EqualTo(newSession.Duration));
            Assert.That(addedSession.VideoLink, Is.EqualTo(newSession.VideoLink));
        });
    }

    [Test]
    public async Task UpdateSession_UpdatesSuccessfully()
    {
        var existingSession = CreateSessionsTemplate()[0];
        await sessionsRepository.CreateSession(existingSession);

        context.Entry(existingSession).State = EntityState.Detached;

        var updatedSession = UpdateSessionTemplate(id: existingSession.Id);

        await sessionsRepository.UpdateSession(updatedSession);
        var retrievedUpdatedSession = await sessionsRepository.GetSessionById(existingSession.Id);

        Assert.That(retrievedUpdatedSession, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(retrievedUpdatedSession.SessionTime, Is.EqualTo(updatedSession.SessionTime));
            Assert.That(retrievedUpdatedSession.Duration, Is.EqualTo(updatedSession.Duration));
            Assert.That(retrievedUpdatedSession.VideoLink, Is.EqualTo(updatedSession.VideoLink));
        });
    }

    [Test]
    public async Task DeleteSession_DeletesSuccessfully()
    {
        var existingSession = CreateSessionsTemplate()[0];
        
        await sessionsRepository.CreateSession(existingSession);
        await sessionsRepository.DeleteSession(existingSession.Id);
        var retrievedNullSession = await sessionsRepository.GetSessionById(existingSession.Id);

        Assert.That(retrievedNullSession, Is.Null);
    }

    #region Private Methods

    private static List<Sessions> CreateSessionsTemplate()
    {
        return new List<Sessions>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                MentorshipRequestId = Guid.NewGuid(),
                SessionTime = DateTime.Now,
                Duration = 30,
                VideoLink = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
            },
            new()
            {
                Id = Guid.NewGuid(),
                MentorshipRequestId = Guid.NewGuid(),
                SessionTime = DateTime.Now,
                Duration = 20,
                VideoLink = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
            }
        };
    }

    private static Sessions UpdateSessionTemplate(Guid id)
    {
        return new Sessions()
        {
            Id = id,
            SessionTime = DateTime.Now.AddDays(3),
            Duration = 40,
            VideoLink = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"
        };
    }

    #endregion
}
