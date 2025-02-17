using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;
using System.Net;

namespace SkillSwap.Tests.Services;

[TestFixture]
public class UserSkillsServiceTests
{
    private Mock<IUsersRepository> usersRepositoryMock;
    private Mock<ISkillsRepository> skillsRepositoryMock;
    private Mock<IUserSkillsRepository> userSkillsRepositoryMock;
    private UserSkillsService userSkillsService;

    [SetUp]
    public void Setup()
    {
        usersRepositoryMock = new Mock<IUsersRepository>();
        skillsRepositoryMock = new Mock<ISkillsRepository>();
        userSkillsRepositoryMock = new Mock<IUserSkillsRepository>();
        userSkillsService = new UserSkillsService(userSkillsRepositoryMock.Object, skillsRepositoryMock.Object);
    }

    [Test]
    public async Task GetUserSkillsByUser_ReturnsSkills()
    {
        var user = CreateUserTemplate();
        var skills = CreateSkillTemplate();

        usersRepositoryMock.Setup(repo => repo.CreateUser(user)).ReturnsAsync(user);
        skillsRepositoryMock.Setup(repo => repo.CreateSkill(skills[0])).ReturnsAsync(skills[0]);
        skillsRepositoryMock.Setup(repo => repo.CreateSkill(skills[1])).ReturnsAsync(skills[1]);
        userSkillsRepositoryMock.Setup(repo => repo.GetUserSkillsByUser(user.Id)).ReturnsAsync(skills);

        var result = await userSkillsService.GetUserSkillsByUser(user.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(skills[0].Id));
            Assert.That(result[0].Name, Is.EqualTo(skills[0].Name));
            Assert.That(result[0].Description, Is.EqualTo(skills[0].Description));
        });
    }

    [Test]
    public async Task UserHasSkill_ReturnsTrue()
    {
        var user = CreateUserTemplate();
        var skill = CreateSkillTemplate()[0];

        usersRepositoryMock.Setup(repo => repo.CreateUser(user)).ReturnsAsync(user);
        skillsRepositoryMock.Setup(repo => repo.CreateSkill(skill)).ReturnsAsync(skill);
        userSkillsRepositoryMock.Setup(repo => repo.UserHasSkill(user.Id, skill.Id)).ReturnsAsync(true);

        var result = await userSkillsService.UserHasSkill(user.Id, skill.Id);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task UserHasSkill_ReturnsFalse()
    {
        var user = CreateUserTemplate();
        var skill = CreateSkillTemplate()[0];

        usersRepositoryMock.Setup(repo => repo.CreateUser(user)).ReturnsAsync(user);
        skillsRepositoryMock.Setup(repo => repo.CreateSkill(skill)).ReturnsAsync(skill);
        userSkillsRepositoryMock.Setup(repo => repo.UserHasSkill(user.Id, skill.Id)).ReturnsAsync(false);

        var result = await userSkillsService.UserHasSkill(user.Id, skill.Id);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CreateUserSkill_CreatesSuccessfully()
    {
        var user = CreateUserTemplate();
        var skill = CreateSkillTemplate()[0];
        var userSkills = new List<Skills>();

        usersRepositoryMock.Setup(repo => repo.CreateUser(user)).ReturnsAsync(user);
        usersRepositoryMock.Setup(repo => repo.GetUserById(user.Id)).ReturnsAsync(user);
        userSkillsRepositoryMock.Setup(repo => repo.UserExists(user.Id)).ReturnsAsync(true);
        skillsRepositoryMock.Setup(repo => repo.GetSkillById(skill.Id)).ReturnsAsync(skill);
        userSkillsRepositoryMock.Setup(repo => repo.SkillExists(skill.Id)).ReturnsAsync(true);
        userSkillsRepositoryMock.Setup(repo => repo.AddSkillToUser(user.Id, skill)).Callback(() => userSkills.Add(skill));
        userSkillsRepositoryMock.Setup(repo => repo.GetUserSkillsByUser(user.Id)).ReturnsAsync(userSkills);

        await userSkillsService.CreateUserSkill(user.Id, skill.Id);
        var result = await userSkillsService.GetUserSkillsByUser(user.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(skill.Id));
            Assert.That(result[0].Name, Is.EqualTo(skill.Name));
            Assert.That(result[0].Description, Is.EqualTo(skill.Description));
        });
    }

    [Test]
    public void CreateUserSkill_ReturnsNotFoundException_WhenUserNotFound()
    {
        var skill = CreateSkillTemplate()[0];

        usersRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>())).ReturnsAsync((Users)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await userSkillsService.CreateUserSkill(Guid.NewGuid(), skill.Id));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("User not found."));
        });
    }

    [Test]
    public void CreateUserSkill_ReturnsNotFoundException_WhenSkillNotFound()
    {
        var user = CreateUserTemplate();

        usersRepositoryMock.Setup(repo => repo.CreateUser(user)).ReturnsAsync(user);
        usersRepositoryMock.Setup(repo => repo.GetUserById(user.Id)).ReturnsAsync(user);
        userSkillsRepositoryMock.Setup(repo => repo.UserExists(user.Id)).ReturnsAsync(true);
        skillsRepositoryMock.Setup(repo => repo.GetSkillById(It.IsAny<Guid>())).ReturnsAsync((Skills)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await userSkillsService.CreateUserSkill(user.Id, Guid.NewGuid()));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("Skill not found."));
        });
    }

    [Test]
    public async Task DeleteUserSkill_DeletesSuccessfully()
    {
        var user = CreateUserTemplate();
        var skill = CreateSkillTemplate()[0];
        var userSkills = new List<Skills>();

        usersRepositoryMock.Setup(repo => repo.CreateUser(user)).ReturnsAsync(user);
        usersRepositoryMock.Setup(repo => repo.GetUserById(user.Id)).ReturnsAsync(user);
        userSkillsRepositoryMock.Setup(repo => repo.UserExists(user.Id)).ReturnsAsync(true);
        skillsRepositoryMock.Setup(repo => repo.GetSkillById(skill.Id)).ReturnsAsync(skill);
        userSkillsRepositoryMock.Setup(repo => repo.SkillExists(skill.Id)).ReturnsAsync(true);
        userSkillsRepositoryMock.Setup(repo => repo.AddSkillToUser(user.Id, skill)).Callback(() => userSkills.Add(skill));
        userSkillsRepositoryMock.Setup(repo => repo.GetUserSkillsByUser(user.Id)).ReturnsAsync(userSkills);
        userSkillsRepositoryMock.Setup(repo => repo.RemoveSkillFromUser(user.Id, skill)).Callback(() => userSkills.Remove(skill)); 

        await userSkillsService.CreateUserSkill(user.Id, skill.Id);
        await userSkillsService.DeleteUserSkill(user.Id, skill.Id);
        var result = await userSkillsService.GetUserSkillsByUser(user.Id);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void DeleteUserSkill_ReturnsNotFoundException_WhenUserNotFound()
    {
        var skill = CreateSkillTemplate()[0];
        var userSkills = new List<Skills>();

        skillsRepositoryMock.Setup(repo => repo.GetSkillById(skill.Id)).ReturnsAsync(skill);
        userSkillsRepositoryMock.Setup(repo => repo.SkillExists(skill.Id)).ReturnsAsync(true);
        userSkillsRepositoryMock.Setup(repo => repo.AddSkillToUser(Guid.NewGuid(), skill)).Callback(() => userSkills.Add(skill));
        userSkillsRepositoryMock.Setup(repo => repo.GetUserSkillsByUser(Guid.NewGuid())).ReturnsAsync(userSkills);
        userSkillsRepositoryMock.Setup(repo => repo.RemoveSkillFromUser(Guid.NewGuid(), skill)).Callback(() => userSkills.Remove(skill));

        var exception = Assert.ThrowsAsync<CustomException>(async () => await userSkillsService.DeleteUserSkill(Guid.NewGuid(), skill.Id));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("User not found."));
        });
    }

    #region Private Methods

    private static Users CreateUserTemplate()
    {
        return new Users()
        {
            Id = Guid.NewGuid(),
            Name = "User Test",
            Email = "usertest@gmail.com",
            Password = PasswordHasher.HashPassword("User@Test-123"),
            Balance = 149.99m
        };
    }

    private static List<Skills> CreateSkillTemplate()
    {
        return new List<Skills>()
        {
            new() 
            {
                Id = Guid.NewGuid(),
                Name = "Skill 1",
                Description = "Skill 1 description"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Skill 2",
                Description = "Skill 2 description"
            }
        };
    }

    #endregion
}
