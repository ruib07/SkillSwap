using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories;

namespace SkillSwap.Tests.Repositories;

[TestFixture]
public class UserSkillsRepositoryTests
{
    private UserSkillsRepository userSkillsRepository;
    private SkillSwapDbContext context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SkillSwapDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

        context = new SkillSwapDbContext(options);
        userSkillsRepository = new UserSkillsRepository(context);
    }

    [TearDown]
    public void TearDown()
    {
        context.Dispose();
    }

    [Test]
    public async Task GetUserSkillsByUser_ReturnsUserSkills()
    {
        var user = CreateUserTemplate();
        var skills = CreateSkillTemplate();
        context.Users.Add(user);
        context.Skills.AddRange(skills);
        await context.SaveChangesAsync();

        await userSkillsRepository.AddSkillToUser(user.Id, skills[0]);
        var retrivedUserSkill = await userSkillsRepository.GetUserSkillsByUser(user.Id);

        Assert.That(retrivedUserSkill, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(retrivedUserSkill, Has.Count.EqualTo(1));
            Assert.That(retrivedUserSkill[0].Id, Is.EqualTo(skills[0].Id));
            Assert.That(retrivedUserSkill[0].Name, Is.EqualTo(skills[0].Name));
            Assert.That(retrivedUserSkill[0].Description, Is.EqualTo(skills[0].Description));
        });
    }

    [Test]
    public async Task UserHasSkill_ReturnsTrue_WhenUserHasSkill()
    {
        var user = CreateUserTemplate();
        var skill = CreateSkillTemplate()[0];

        context.Users.Add(user);
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        await userSkillsRepository.AddSkillToUser(user.Id, skill);
        var trueResponse = await userSkillsRepository.UserHasSkill(user.Id, skill.Id);

        Assert.That(trueResponse, Is.True); 
    }

    [Test]
    public async Task UserHasSkill_ReturnsFalse_WhenUserDontHaveSkill()
    {
        var user = CreateUserTemplate();
        var skill = CreateSkillTemplate()[0];

        context.Users.Add(user);
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        var falseResponse = await userSkillsRepository.UserHasSkill(user.Id, skill.Id);

        Assert.That(falseResponse, Is.False);

    }

    [Test]
    public async Task UserExists_ReturnsTrue_WhenUserExists()
    {
        var user = CreateUserTemplate();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var userExists = await userSkillsRepository.UserExists(user.Id);

        Assert.That(userExists, Is.True);
    }

    [Test]
    public async Task UserExists_ReturnsFalse_WhenUserDoesntExist()
    {
        var userDontExist = await userSkillsRepository.UserExists(Guid.NewGuid());

        Assert.That(userDontExist, Is.False);
    }

    [Test]
    public async Task SkillExists_ReturnsTrue_WhenSkillExists()
    {
        var skills = CreateSkillTemplate();
        context.Skills.AddRange(skills);
        await context.SaveChangesAsync();

        var skillExists = await userSkillsRepository.SkillExists(skills[0].Id);

        Assert.That(skillExists, Is.True);
    }

    [Test]
    public async Task SkillExists_ReturnsFalse_WhenSkillDoesntExist()
    {
        var skillDontExist = await userSkillsRepository.SkillExists(Guid.NewGuid());

        Assert.That(skillDontExist, Is.False);
    }

    [Test]
    public async Task AddSkillToUser_AddsSkill()
    {
        var user = CreateUserTemplate();
        var skill = CreateSkillTemplate()[1];
        context.Users.Add(user);
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        await userSkillsRepository.AddSkillToUser(user.Id, skill);
        var retrivedUserSkill = await userSkillsRepository.GetUserSkillsByUser(user.Id);

        Assert.That(retrivedUserSkill, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(retrivedUserSkill[0].Id, Is.EqualTo(skill.Id));
            Assert.That(retrivedUserSkill[0].Name, Is.EqualTo(skill.Name));
            Assert.That(retrivedUserSkill[0].Description, Is.EqualTo(skill.Description));
        });
    }

    [Test]
    public async Task RemoveSkillFromUser_RemovesSuccessfully()
    {
        var user = CreateUserTemplate();
        var skill = CreateSkillTemplate()[0];
        context.Users.Add(user);
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        await userSkillsRepository.AddSkillToUser(user.Id, skill);
        await userSkillsRepository.RemoveSkillFromUser(user.Id, skill);
        var retrievedNullSkill = await userSkillsRepository.GetUserSkillsByUser(user.Id);

        Assert.That(retrievedNullSkill, Is.Empty);
    }

    #region Private Methods

    private static Users CreateUserTemplate()
    {
        return new Users()
        {
            Id = Guid.NewGuid(),
            Name = "User 1",
            Email = "user1test@gmail.com",
            Password = PasswordHasher.HashPassword("User1@Test-123"),
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
                Name = "Skill1",
                Description = "Skill1 Description"
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Skill2",
                Description = "Skill2 Description"
            }
        };
    }

    #endregion
}
