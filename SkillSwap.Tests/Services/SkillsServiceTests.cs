using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;
using System.Net;

namespace SkillSwap.Tests.Services;

[TestFixture]
public class SkillsServiceTests
{
    private Mock<ISkillsRepository> skillsRepositoryMock;
    private SkillsService skillsService;

    [SetUp]
    public void Setup()
    {
        skillsRepositoryMock = new Mock<ISkillsRepository>();
        skillsService = new SkillsService(skillsRepositoryMock.Object);
    }

    [Test]
    public async Task GetAllSkills_ReturnsSkills()
    {
        var skills = CreateSkillsTemplate();

        skillsRepositoryMock.Setup(repo => repo.GetAllSkills()).ReturnsAsync(skills);

        var result = await skillsService.GetAllSkills();

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0].Id, Is.EqualTo(skills[0].Id));
            Assert.That(result[0].Name, Is.EqualTo(skills[0].Name));
            Assert.That(result[0].Description, Is.EqualTo(skills[0].Description));
            Assert.That(result[1].Id, Is.EqualTo(skills[1].Id));
            Assert.That(result[1].Name, Is.EqualTo(skills[1].Name));
            Assert.That(result[1].Description, Is.EqualTo(skills[1].Description));
        });
    }

    [Test]
    public async Task GetSkillById_ReturnsSkill()
    {
        var skill = CreateSkillsTemplate()[0];

        skillsRepositoryMock.Setup(repo => repo.GetSkillById(skill.Id)).ReturnsAsync(skill);

        var result = await skillsService.GetSkillById(skill.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(skill.Id));
            Assert.That(result.Name, Is.EqualTo(skill.Name));
            Assert.That(result.Description, Is.EqualTo(skill.Description));
        });
    }

    [Test]
    public void GetSkillById_ReturnsNotFoundException_WhenSkillIdDontExist()
    {
        skillsRepositoryMock.Setup(repo => repo.GetSkillById(It.IsAny<Guid>())).ReturnsAsync((Skills)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await skillsService.GetSkillById(Guid.NewGuid()));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("Skill not found."));
        });
    }

    [Test]
    public async Task CreateSkill_CreatesSuccessfully()
    {
        var skill = CreateSkillsTemplate()[0];

        skillsRepositoryMock.Setup(repo => repo.CreateSkill(skill)).ReturnsAsync(skill);

        var result = await skillsService.CreateSkill(skill);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(skill.Id));
            Assert.That(result.Name, Is.EqualTo(skill.Name));
            Assert.That(result.Description, Is.EqualTo(skill.Description));
        });
    }

    [Test]
    public void CreateSkill_ReturnsBadRequestException_WhenNameAndDescriptionAreEmpty()
    {
        var skill = InvalidSkillTemplate(string.Empty, string.Empty);

        skillsRepositoryMock.Setup(repo => repo.CreateSkill(skill)).ReturnsAsync(skill);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await skillsService.CreateSkill(skill));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(exception.Message, Is.EqualTo("Name and description are required."));
        });
    }

    [Test]
    public void CreateSkill_ReturnsConflictException_WhenNameAlreadyExists()
    {
        var validSkill = CreateSkillsTemplate()[0];
        var invalidSkill = InvalidSkillTemplate("Skill1", "Test invalid skill");

        skillsRepositoryMock.Setup(repo => repo.CreateSkill(validSkill)).ReturnsAsync(validSkill);
        skillsRepositoryMock.Setup(repo => repo.EnsureSkillNameIsUnique(invalidSkill.Name, null)).ReturnsAsync(true);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await skillsService.CreateSkill(invalidSkill));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(exception.Message, Is.EqualTo("Skill Name already exists."));
        });
    }

    [Test]
    public async Task UpdateSkill_UpdatesSuccessfully()
    {
        var skill = CreateSkillsTemplate()[0];
        var updateSkill = UpdateSkillTemplate(skill.Id);

        skillsRepositoryMock.Setup(repo => repo.CreateSkill(skill)).ReturnsAsync(skill);
        skillsRepositoryMock.Setup(repo => repo.UpdateSkill(updateSkill)).Returns(Task.CompletedTask);
        skillsRepositoryMock.Setup(repo => repo.GetSkillById(skill.Id)).ReturnsAsync(skill);

        await skillsService.UpdateSkill(skill.Id, updateSkill);
        var result = await skillsService.GetSkillById(skill.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(updateSkill.Id));
            Assert.That(result.Name, Is.EqualTo(updateSkill.Name));
            Assert.That(result.Description, Is.EqualTo(updateSkill.Description));
        });
    }

    [Test]
    public async Task DeleteSkill_DeletesSuccessfully()
    {
        var skill = CreateSkillsTemplate()[0];

        skillsRepositoryMock.Setup(repo => repo.CreateSkill(skill)).ReturnsAsync(skill);
        skillsRepositoryMock.Setup(repo => repo.DeleteSkill(skill.Id)).Returns(Task.CompletedTask);
        skillsRepositoryMock.Setup(repo => repo.GetSkillById(skill.Id)).ReturnsAsync((Skills)null);

        await skillsService.CreateSkill(skill);
        await skillsService.DeleteSkill(skill.Id);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await skillsService.GetSkillById(skill.Id));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("Skill not found."));
        });
    }

    #region Private Methods

    private static List<Skills> CreateSkillsTemplate()
    {
        return new List<Skills>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Skill 1",
                Description = "Skill 1 description",
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Skill 2",
                Description = "Skill 2 description",
            }
        };
    }

    private static Skills InvalidSkillTemplate(string name, string description)
    {
        return new Skills()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
        };
    }

    private static Skills UpdateSkillTemplate(Guid id)
    {
        return new Skills()
        {
            Id = id,
            Name = "Skill1 Updated",
            Description = "Skill1 description updated",
        };
    }

    #endregion
}
