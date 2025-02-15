using Microsoft.AspNetCore.Mvc;
using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Controllers;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;
using SkillSwap.Services.Services.Email;
using static SkillSwap.Server.Models.RecoverPassword;
using static SkillSwap.Server.Models.Responses;
using static SkillSwap.Server.Models.UpdateBalance;

namespace SkillSwap.Tests.Controllers;

[TestFixture]
public class UsersControllerTests
{
    private Mock<IUsersRepository> usersRepositoryMock;
    private UsersService usersService;
    private Mock<IEmailPasswordResets> emailServiceMock;
    private UsersController usersController;

    [SetUp]
    public void Setup()
    {
        usersRepositoryMock = new Mock<IUsersRepository>();
        usersService = new UsersService(usersRepositoryMock.Object);
        emailServiceMock = new Mock<IEmailPasswordResets>();
        usersController = new UsersController(usersService, emailServiceMock.Object);
    }

    [Test]
    public async Task GetUserById_ReturnsOkResult_WithUser()
    {
        var mockUser = CreateUserTemplate()[0];

        usersRepositoryMock.Setup(repo => repo.GetUserById(mockUser.Id)).ReturnsAsync(mockUser);
        var result = await usersController.GetUserById(mockUser.Id);
        var okResult = result as OkObjectResult;
        var response = okResult.Value as Users;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response.Id, Is.EqualTo(mockUser.Id));
            Assert.That(response.Name, Is.EqualTo(mockUser.Name));
            Assert.That(response.Email, Is.EqualTo(mockUser.Email));
            Assert.That(response.Balance, Is.EqualTo(mockUser.Balance));
        });
    }

    [Test]
    public async Task GetMentors_ReturnsOkResult_WithMentors()
    {
        var mentors = CreateUserTemplate();

        usersRepositoryMock.Setup(repo => repo.GetMentors()).ReturnsAsync(mentors);

        var result = await usersController.GetMentors();
        var okResult = result.Result as OkObjectResult;
        var response = okResult.Value as List<Users>;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task CreateUser_ReturnsCreatedResult_WithValidUser()
    {
        var newUser = CreateUserTemplate()[0];

        usersRepositoryMock.Setup(repo => repo.CreateUser(It.IsAny<Users>())).ReturnsAsync(newUser);
        usersRepositoryMock.Setup(repo => repo.GetUserByEmail(newUser.Email)).ReturnsAsync((Users)null);

        var result = await usersController.CreateUser(newUser);
        var createdResult = result.Result as ObjectResult;
        var response = createdResult.Value as CreationResponse;

        Assert.Multiple(() =>
        {
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
            Assert.That(response.Message, Is.EqualTo("User created successfully."));
            Assert.That(response.Id, Is.EqualTo(newUser.Id));
        });
    }

    [Test]
    public async Task RecoverPassword_ReturnsOk_WhenTokenGeneratedAndEmailSent()
    {
        var newUser = CreateUserTemplate()[0];
        var passwordResetToken = "generated_token";

        usersRepositoryMock.Setup(repo => repo.GetUserByEmail(newUser.Email)).ReturnsAsync(newUser);
        usersRepositoryMock.Setup(repo => repo.GeneratePasswordResetToken(newUser.Id)).ReturnsAsync(passwordResetToken);
        emailServiceMock.Setup(repo => repo.SendPasswordResetEmail(newUser.Email, passwordResetToken)).Returns(Task.CompletedTask);

        var recoverPasswordRequest = new RecoverPasswordRequest { Email = newUser.Email };
        var recoverPasswordEmailResult = await usersController.RecoverPasswordSendEmail(recoverPasswordRequest);
        var recoverPasswordResponse = recoverPasswordEmailResult as OkResult;

        Assert.That(recoverPasswordResponse.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task RecoverPassword_ReturnsOk_WhenInvalidEmail()
    {
        var request = new RecoverPasswordRequest() { Email = "testuser@gmail.com" };
        var token = "";

        usersRepositoryMock.Setup(repo => repo.GetUserByEmail(request.Email)).ReturnsAsync((Users)null);
        emailServiceMock.Setup(repo => repo.SendPasswordResetEmail(request.Email, token)).Returns(Task.CompletedTask);

        var result = await usersController.RecoverPasswordSendEmail(request);
        var okResult = result as OkResult;

        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task UpdatePassword_ReturnsOkResult_WhenPasswordIsUpdatedSuccessfully()
    {
        var request = UpdatePasswordRequestTemplate(confirmNewPassword: "New@Password-123");
        var user = CreateUserTemplate()[0];
        var token = new PasswordResetToken { Token = request.Token, User = user };

        usersRepositoryMock.Setup(repo => repo.GetPasswordResetToken(request.Token)).ReturnsAsync(token);
        usersRepositoryMock.Setup(repo => repo.UpdateUser(It.IsAny<Users>())).Returns(Task.CompletedTask);
        usersRepositoryMock.Setup(repo => repo.RemovePasswordResetToken(token)).Returns(Task.CompletedTask);

        var result = await usersController.UpdatePassword(request);
        var okResult = result as OkResult;

        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public async Task UpdateUser_ReturnsOkResult_WithUpdatedUser()
    {
        var userToUpdate = CreateUserTemplate()[0];
        var updatedUser = UpdateUserTemplate(email: "updateduser@gmail.com");

        usersRepositoryMock.Setup(repo => repo.GetUserById(userToUpdate.Id)).ReturnsAsync(userToUpdate);
        usersRepositoryMock.Setup(repo => repo.UpdateUser(It.IsAny<Users>())).Returns(Task.CompletedTask);

        var result = await usersController.UpdateUser(userToUpdate.Id, updatedUser);
        var okResult = result as OkObjectResult;

        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo("User updated successfully."));
        });
    }

    [Test]
    public async Task UpdateBalance_ReturnsOkResult_WhenBalanceIsProvided()
    {
        var user = CreateUserTemplate()[0];
        var updatedBalance = 249.99m;
        var request = new UpdateBalanceRequest() { Balance = updatedBalance };

        usersRepositoryMock.Setup(repo => repo.CreateUser(It.IsAny<Users>())).ReturnsAsync(user);
        usersRepositoryMock.Setup(repo => repo.GetUserById(user.Id)).ReturnsAsync(user);
        usersRepositoryMock.Setup(repo => repo.UpdateBalance(user, request.Balance.Value))
                           .ReturnsAsync(updatedBalance);

        var result = await usersController.UpdateBalance(user.Id, request);
        var okResult = result as OkObjectResult;
        var response = okResult.Value as UpdateBalanceResponse;

        Assert.That(result, Is.Not.Null);
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(response.UpdatedBalance, Is.EqualTo(updatedBalance));
        Assert.That(response.Message, Is.EqualTo("Balance updated successfully."));
    }

    [Test]
    public async Task UpdateBalance_ReturnsBadRequest_WhenBalanceIsNotProvided()
    {
        var user = CreateUserTemplate()[0];
        var request = new UpdateBalanceRequest() { Balance = null };

        usersRepositoryMock.Setup(repo => repo.CreateUser(It.IsAny<Users>())).ReturnsAsync(user);
        usersRepositoryMock.Setup(repo => repo.GetUserById(user.Id)).ReturnsAsync(user);

        var result = await usersController.UpdateBalance(user.Id, request);
        var badRequestResult = result as BadRequestObjectResult;
        var response = badRequestResult.Value as UpdateBalanceBadRequest;

        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(response.Message, Is.EqualTo("Balance is required."));
            Assert.That(response.StatusCode, Is.EqualTo(400));
        });
    }

    [Test]
    public async Task DeleteUser_ReturnsNoContent_WhenUserIsDeleted()
    {
        var user = CreateUserTemplate()[0];

        usersRepositoryMock.Setup(repo => repo.GetUserById(user.Id)).ReturnsAsync(user);
        usersRepositoryMock.Setup(repo => repo.DeleteUser(user.Id)).Returns(Task.CompletedTask);

        var result = await usersController.DeleteUser(user.Id);
        var noContentResult = result as NoContentResult;

        Assert.That(noContentResult.StatusCode, Is.EqualTo(204));
    }

    #region Private Methods

    private static List<Users> CreateUserTemplate()
    {
        return new List<Users>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "User Test",
                Email = "usertest@gmail.com",
                Password = PasswordHasher.HashPassword("User1@Test-123"),
                Balance = 149.99m,
                IsMentor = true
            }
        };
    }

    private static UpdatePasswordRequest UpdatePasswordRequestTemplate(string confirmNewPassword)
    {
        return new UpdatePasswordRequest()
        {
            Token = "validToken",
            NewPassword = "New@Password-123",
            ConfirmNewPassword = confirmNewPassword
        };
    }

    private static Users UpdateUserTemplate(string email)
    {
        return new Users()
        {
            Name = "Updated User",
            Email = email,
            Password = PasswordHasher.HashPassword("Updated@User-123"),
            Bio = "Now I have a bio",
            ProfilePicture = "https://i.pinimg.com/736x/d6/82/57/d682577ac42e84125461689aa9b4623a.jpg",
            Balance = 299.99m,
            IsMentor = false
        };
    }

    #endregion
}
