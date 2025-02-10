using Microsoft.AspNetCore.Mvc;
using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Controllers;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;
using static SkillSwap.Server.Models.Responses;

namespace SkillSwap.Tests.Controllers;

[TestFixture]
public class SkillsControllerTests
{
    private Mock<ISkillsRepository> skillsRepositoryMock;
    private SkillsService skillsService;
    private SkillsController skillsController;

    [SetUp]
    public void Setup()
    {
        skillsRepositoryMock = new Mock<ISkillsRepository>();
        skillsService = new SkillsService(skillsRepositoryMock.Object);
        skillsController = new SkillsController(skillsService);
    }

    [Test]
    public async Task GetAllSkills_ReturnsOkResult_WithListOfSkills()
    {
        var skillsList = CreateListSkillsTemplate();

        skillsRepositoryMock.Setup(repo => repo.GetAllSkills()).ReturnsAsync(skillsList);

        var result = await skillsController.GetAllSkills();
        var okResult = result.Result as OkObjectResult;
        var response = okResult.Value as List<Skills>;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public async Task GetSkillById_ReturnsOkResult_WithSkill()
    {
        var mockSkill = CreateListSkillsTemplate()[0];

        skillsRepositoryMock.Setup(repo => repo.GetSkillById(mockSkill.Id)).ReturnsAsync(mockSkill);

        var result = await skillsController.GetSkillById(mockSkill.Id);
        var okResult = result as OkObjectResult;
        var response = okResult.Value as Skills;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response.Name, Is.EqualTo("Skill1"));
            Assert.That(response.Description, Is.EqualTo("Skill1 Description"));
        });
    }

    [Test]
    public async Task CreateSkill_ReturnsCreatedResult_WithValidSkill()
    {
        var newSkill = CreateListSkillsTemplate()[0];

        skillsRepositoryMock.Setup(repo => repo.CreateSkill(It.IsAny<Skills>())).ReturnsAsync(newSkill);

        var result = await skillsController.CreateSkill(newSkill);
        var createdResult = result.Result as ObjectResult;
        var response = createdResult.Value as CreationResponse;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
            Assert.That(response.Message, Is.EqualTo("Skill created successfully."));
            Assert.That(response.Id, Is.EqualTo(newSkill.Id));
        });
    }

    [Test]
    public async Task UpdateSkill_ReturnsOkResult_WithUpdatedSkill()
    {
        var skillToUpdate = CreateListSkillsTemplate()[0];
        var updatedSkill = UpdateSkillTemplate();

        skillsRepositoryMock.Setup(repo => repo.GetSkillById(skillToUpdate.Id)).ReturnsAsync(skillToUpdate);
        skillsRepositoryMock.Setup(repo => repo.UpdateSkill(It.IsAny<Skills>())).Returns(Task.CompletedTask);

        var result = await skillsController.UpdateSkill(skillToUpdate.Id, updatedSkill);
        var okResult = result as OkObjectResult;

        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo("Skill updated successfully."));
        });
    }

    [Test]
    public async Task DeleteSkill_ReturnsNoContent_WhenSkillIsDeleted()
    {
        var skill = CreateListSkillsTemplate()[0];

        skillsRepositoryMock.Setup(repo => repo.GetSkillById(skill.Id)).ReturnsAsync(skill);
        skillsRepositoryMock.Setup(repo => repo.DeleteSkill(skill.Id)).Returns(Task.CompletedTask);

        var result = await skillsController.DeleteSkill(skill.Id);
        var noContentResult = result as NoContentResult;

        Assert.That(noContentResult.StatusCode, Is.EqualTo(204));
    }

    #region Private Methods

    private static List<Skills> CreateListSkillsTemplate()
    {
        return new List<Skills>()
        {
            new() { Id = Guid.NewGuid(), Name = "Skill1", Description = "Skill1 Description" },
            new() { Id = Guid.NewGuid(), Name = "Skill2", Description = "Skill2 Description" }
        };
    }

    private static Skills UpdateSkillTemplate()
    {
        return new Skills()
        {
            Name = "Skill1 Updated",
            Description = "Skill1 Updated Description"
        };
    }

    #endregion
}
