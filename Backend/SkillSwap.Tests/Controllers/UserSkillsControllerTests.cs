using Microsoft.AspNetCore.Mvc;
using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Controllers;
using SkillSwap.Server.Models.DTOs;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;

namespace SkillSwap.Tests.Controllers;

[TestFixture]
public class UserSkillsControllerTests
{
    private Mock<IUserSkillsRepository> userSkillsRepositoryMock;
    private Mock<ISkillsRepository> skillsRepositoryMock;
    private UserSkillsService userSkillsService;
    private UserSkillsController userSkillsController;

    [SetUp]
    public void Setup()
    {
        userSkillsRepositoryMock = new Mock<IUserSkillsRepository>();
        skillsRepositoryMock = new Mock<ISkillsRepository>();
        userSkillsService = new UserSkillsService(userSkillsRepositoryMock.Object, skillsRepositoryMock.Object);
        userSkillsController = new UserSkillsController(userSkillsService);
    }

    [Test]
    public async Task GetUserSkillsByUser_ReturnsOkResult_WithUserSkills()
    {
        var userId = Guid.NewGuid();
        var skillsList = CreateSkillsTemplate();

        userSkillsRepositoryMock.Setup(repo => repo.GetUserSkillsByUser(userId)).ReturnsAsync(skillsList);

        var result = await userSkillsController.GetUserSkillsByUser(userId);
        var okResult = result.Result as OkObjectResult;
        var response = okResult?.Value as List<Skills>;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response, Has.Count.EqualTo(2));
            Assert.That(response[0].Name, Is.EqualTo(skillsList[0].Name));
            Assert.That(response[1].Name, Is.EqualTo(skillsList[1].Name));
        });
    }

    [Test]
    public async Task UserHasSkill_ReturnsTrue_WhenUserHasSkill()
    {
        var userId = Guid.NewGuid();
        var skillId = Guid.NewGuid();

        userSkillsRepositoryMock.Setup(repo => repo.UserHasSkill(userId, skillId)).ReturnsAsync(true);

        var result = await userSkillsController.UserHasSkill(userId, skillId);
        var okResult = result.Result as OkObjectResult;
        var response = okResult?.Value as bool?;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response, Is.True);
        });
    }

    [Test]
    public async Task UserHasSkill_ReturnsFalse_WhenUserDoesNotHaveSkill()
    {
        var userId = Guid.NewGuid();
        var skillId = Guid.NewGuid();

        userSkillsRepositoryMock.Setup(repo => repo.UserHasSkill(userId, skillId)).ReturnsAsync(false);

        var result = await userSkillsController.UserHasSkill(userId, skillId);
        var okResult = result.Result as OkObjectResult;
        var response = okResult?.Value as bool?;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response, Is.False);
        });
    }

    [Test]
    public async Task CreateUserSkill_ReturnsCreatedResult_WithValidUserSkill()
    {
        var userSkillDto = CreateUserSkillDTOTemplate();
        var skill = CreateSkillsTemplate()[0];

        userSkillsRepositoryMock.Setup(repo => repo.UserExists(userSkillDto.UserId)).ReturnsAsync(true);
        userSkillsRepositoryMock.Setup(repo => repo.SkillExists(userSkillDto.SkillId)).ReturnsAsync(true);
        userSkillsRepositoryMock.Setup(repo => repo.GetUserSkillsByUser(userSkillDto.UserId))
                                 .ReturnsAsync(new List<Skills>());
        skillsRepositoryMock.Setup(repo => repo.GetSkillById(userSkillDto.SkillId)).ReturnsAsync(skill);
        userSkillsRepositoryMock.Setup(repo => repo.AddSkillToUser(userSkillDto.UserId, It.IsAny<Skills>()))
                                .Returns(Task.CompletedTask);

        var result = await userSkillsController.CreateUserSkill(userSkillDto);
        var createdResult = result as ObjectResult;

        Assert.That(createdResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
            Assert.That(createdResult.Value, Is.EqualTo("User skill created successfully."));
        });
    }

    [Test]
    public async Task DeleteUserSkill_ReturnsNoContent_WhenUserSkillIsDeleted()
    {
        var userId = Guid.NewGuid();
        var skill = CreateSkillsTemplate()[0];

        var userSkillDto = new UserSkillDto(UserId: userId, SkillId: skill.Id);

        userSkillsRepositoryMock.Setup(repo => repo.UserExists(userId)).ReturnsAsync(true);
        userSkillsRepositoryMock.Setup(repo => repo.SkillExists(skill.Id)).ReturnsAsync(true);
        skillsRepositoryMock.Setup(repo => repo.GetSkillById(skill.Id)).ReturnsAsync(skill);
        userSkillsRepositoryMock.Setup(repo => repo.GetUserSkillsByUser(userId)).ReturnsAsync(new List<Skills>());
        userSkillsRepositoryMock.Setup(repo => repo.AddSkillToUser(userId, skill)).Returns(Task.CompletedTask);

        await userSkillsController.CreateUserSkill(userSkillDto);

        userSkillsRepositoryMock.Setup(repo => repo.GetUserSkillsByUser(userId)).ReturnsAsync(new List<Skills> { skill });
        userSkillsRepositoryMock.Setup(repo => repo.RemoveSkillFromUser(userId, skill)).Returns(Task.CompletedTask);

        var result = await userSkillsController.DeleteUserSkill(userId, skill.Id);
        var noContentResult = result as NoContentResult;

        Assert.That(noContentResult, Is.Not.Null);
        Assert.That(noContentResult.StatusCode, Is.EqualTo(204));
    }

    #region Private Methods

    private static List<Skills> CreateSkillsTemplate()
    {
        return new List<Skills>()
        {
            new() { Id = Guid.NewGuid(), Name = "C#" },
            new() { Id = Guid.NewGuid(), Name = "JavaScript" }
        };
    }

    private static UserSkillDto CreateUserSkillDTOTemplate()
    {
        return new UserSkillDto(UserId: Guid.NewGuid(), SkillId: Guid.NewGuid());
    }

    #endregion
}
