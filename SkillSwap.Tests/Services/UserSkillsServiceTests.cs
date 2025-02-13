using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;

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
        skillsRepositoryMock.Setup(repo => repo.CreateSkill(It.IsAny<Skills>())).ReturnsAsync((Skills s) => s);
        userSkillsRepositoryMock.Setup(repo => repo.GetUserSkillsByUser(user.Id)).ReturnsAsync(skills);

        var result = await userSkillsService.GetUserSkillsByUser(user.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(skills[0].Id));
            Assert.That(result[0].Name, Is.EqualTo(skills[0].Name));
            Assert.That(result[0].Description, Is.EqualTo(skills[0].Description));
        });
    }

    [Test]
    public async Task CreateUserSkill_CreatesSuccessfully()
    {
        var user = CreateUserTemplate();
        var skill = CreateSkillTemplate()[0];

        usersRepositoryMock.Setup(repo => repo.CreateUser(user)).ReturnsAsync(user);
        skillsRepositoryMock.Setup(repo => repo.CreateSkill(skill)).ReturnsAsync(skill);
        userSkillsRepositoryMock.Setup(repo => repo.AddSkillToUser(user.Id, skill));

        await userSkillsService.CreateUserSkill(user.Id, skill.Id);
        var result = await userSkillsService.GetUserSkillsByUser(user.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result[0].Id, Is.EqualTo(skill.Id));
            Assert.That(result[0].Name, Is.EqualTo(skill.Name));
            Assert.That(result[0].Description, Is.EqualTo(skill.Description));
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
