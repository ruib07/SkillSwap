using Microsoft.AspNetCore.Mvc;
using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Controllers;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Interfaces;
using SkillSwap.Tests.Helpers;
using System.Net;
using static SkillSwap.Server.Models.RecoverPassword;
using static SkillSwap.Server.Models.Responses;

namespace SkillSwap.Tests.Controllers;

[TestFixture]
public class UsersControllerTests
{
    private Mock<IUsers> usersServiceMock;
    private Mock<IEmailPasswordResets> emailServiceMock;
    private UsersController users;

    [SetUp]
    public void Setup()
    {
        usersServiceMock = new Mock<IUsers>();
        emailServiceMock = new Mock<IEmailPasswordResets>();
        users = new UsersController(usersServiceMock.Object, emailServiceMock.Object);
    }

    [Test]
    public async Task GetUserById_ReturnsOkResult_WithUser()
    {
        var userId = Guid.NewGuid();
        var mockUser = CreateUserTemplate();

        usersServiceMock.Setup(usm => usm.GetUserById(userId)).ReturnsAsync(mockUser);
        var result = await users.GetUserById(userId);
        var okResult = result as OkObjectResult;
        var response = okResult.Value as Users;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(mockUser.Id));
            Assert.That(response.Name, Is.EqualTo(mockUser.Name));
            Assert.That(response.Email, Is.EqualTo(mockUser.Email));
            Assert.That(response.Balance, Is.EqualTo(mockUser.Balance));
        });
    }

    [Test]
    public async Task CreateUser_ReturnsCreatedResult_WithValidUser()
    {
        var newUser = CreateUserTemplate();
        usersServiceMock.Setup(service => service.CreateUser(It.IsAny<Users>())).ReturnsAsync(newUser);
        var result = await users.CreateUser(newUser);
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
    public async Task CreateUser_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        users.ModelState.AddModelError("Name", "Required");
        var result = await users.CreateUser(new Users());
        var badRequestResult = result.Result as BadRequestObjectResult;

        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.Value, Is.TypeOf<SerializableError>());
    }

    [Test]
    public async Task RecoverPassword_ReturnsOk_WhenTokenGeneratedAndEmailSent()
    {
        var newUser = CreateUserTemplate();
        usersServiceMock.Setup(service => service.CreateUser(It.IsAny<Users>())).ReturnsAsync(newUser);

        var passwordResetToken = "generated_token";
        usersServiceMock.Setup(usm => usm.GeneratePasswordResetToken(newUser.Email)).ReturnsAsync(passwordResetToken);
        emailServiceMock.Setup(esm => esm.SendPasswordResetEmail(newUser.Email, passwordResetToken)).Returns(Task.CompletedTask);

        var createUserResult = await users.CreateUser(newUser);
        var createUserObjectResult = createUserResult.Result as ObjectResult;
        var createUserResponse = createUserObjectResult.Value as CreationResponse;

        var recoverPasswordRequest = new RecoverPasswordRequest { Email = newUser.Email };
        var recoverPasswordEmailResult = await users.RecoverPasswordSendEmail(recoverPasswordRequest);
        var recoverPasswordResponse = recoverPasswordEmailResult as OkResult;

        Assert.Multiple(() =>
        {
            Assert.That(createUserObjectResult.StatusCode, Is.EqualTo(201));
            Assert.That(createUserResponse.Message, Is.EqualTo("User created successfully."));
            Assert.That(createUserResponse.Id, Is.EqualTo(newUser.Id));
            Assert.That(recoverPasswordResponse.StatusCode, Is.EqualTo(200));
        });
    }

    [Test]
    public async Task RecoverPassword_ReturnsOk_WhenInvalidEmail()
    {
        var request = new RecoverPasswordRequest() { Email = "testuser@gmail.com" };
        var token = "";

        usersServiceMock.Setup(usm => usm.GeneratePasswordResetToken(request.Email)).ReturnsAsync(token);
        emailServiceMock.Setup(esm => esm.SendPasswordResetEmail(request.Email, token)).Returns(Task.CompletedTask);

        var result = await users.RecoverPasswordSendEmail(request);
        var okResult = result as OkResult;

        Assert.That(okResult.StatusCode, Is.EqualTo(200));  
    }


    [Test]
    public async Task UpdatePassword_ReturnsOkResult_WhenPasswordIsUpdatedSuccessfully()
    {
        var request = UpdatePasswordRequestTemplate(confirmNewPassword: "New@Password-123");

        usersServiceMock.Setup(usm => usm.UpdatePassword(request.Token, request.NewPassword, request.ConfirmNewPassword))
                        .Returns(Task.CompletedTask);

        var result = await users.UpdatePassword(request);
        var okResult = result as OkResult;

        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }

    //[Test]
    //public async Task UpdatePassword_ReturnsBadRequest_WhenPasswordsDoNotMatch()
    //{
    //    var request = UpdatePasswordRequestTemplate(confirmNewPassword: "New@Password-12");

    //    usersServiceMock.Setup(usm => usm.UpdatePassword(request.Token, request.NewPassword, request.ConfirmNewPassword))
    //                    .ThrowsAsync(new CustomException("Passwords do not match.", HttpStatusCode.BadRequest));

    //    var result = await users.UpdatePassword(request);
    //    var badRequestResult = result as BadRequestObjectResult;

    //    Assert.That(badRequestResult, Is.Not.Null);
    //    Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
    //    Assert.That(badRequestResult.Value, Is.EqualTo("Passwords do not match."));
    //}

    [Test]
    public async Task UpdateUser_ReturnsOkResult_WithUpdatedUser()
    {
        var userToUpdate = CreateUserTemplate();
        var updatedUser = UpdateUserTemplate(email: "updateduser@gmail.com");

        usersServiceMock.Setup(usm => usm.UpdateUser(userToUpdate.Id, userToUpdate)).ReturnsAsync(updatedUser);

        var result = await users.UpdateUser(userToUpdate.Id, updatedUser);
        var okResult = result as OkObjectResult;

        Assert.That(okResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo("User updated successfully."));
        });
    }

    //[Test]
    //public async Task UpdateUser_ReturnsNotFound_WhenUserNotFound()
    //{
    //    var userId = Guid.NewGuid();
    //    var updatedUser = UpdateUserTemplate(email: "nonexisting@gmail.com");

    //    usersServiceMock.Setup(usm => usm.UpdateUser(userId, updatedUser))
    //                    .ThrowsAsync(new CustomException("User not found.", HttpStatusCode.NotFound));

    //    var result = await users.UpdateUser(userId, updatedUser);
    //    var notFoundResult = result as NotFoundObjectResult;

    //    Assert.That(notFoundResult, Is.Not.Null);
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
    //        Assert.That(notFoundResult.Value, Is.EqualTo("User not found."));
    //    });
    //}

    //[Test]
    //public async Task UpdateUser_ReturnsBadRequest_WhenEmailAlreadyInUse()
    //{
    //    var userToUpdate = CreateUserTemplate();
    //    var updatedUser = UpdateUserTemplate(email: userToUpdate.Email);

    //    usersServiceMock.Setup(usm => usm.UpdateUser(userToUpdate.Id, updatedUser))
    //                    .ThrowsAsync(new CustomException("Email already in use.", HttpStatusCode.Conflict));

    //    var result = await users.UpdateUser(userToUpdate.Id, userToUpdate);
    //    var badRequestResult = result as BadRequestObjectResult;

    //    Assert.That(badRequestResult, Is.Not.Null);
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(badRequestResult.StatusCode, Is.EqualTo(409));
    //        Assert.That(badRequestResult.Value, Is.EqualTo("Email already in use."));
    //    });
    //}

    [Test]
    public async Task DeleteUser_ReturnsNoContent_WhenUserIsDeleted()
    {
        var userId = Guid.NewGuid();
        usersServiceMock.Setup(service => service.DeleteUser(userId)).Returns(Task.CompletedTask);

        var result = await users.DeleteUser(userId);
        var noContentResult = result as NoContentResult;

        Assert.That(noContentResult.StatusCode, Is.EqualTo(204));
    }

    //[Test]
    //public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
    //{
    //    var userId = Guid.NewGuid();
    //    usersServiceMock.Setup(service => service.DeleteUser(userId))
    //                    .ThrowsAsync(new CustomException("User not found.", HttpStatusCode.NotFound));

    //    var result = await users.DeleteUser(userId);
    //    var notFoundResult = result as NotFoundResult;

    //    Assert.That(notFoundResult, Is.Not.Null);
    //    Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
    //}


    #region Private Methods

    private static Users CreateUserTemplate()
    {
        return new Users()
        {
            Id = Guid.NewGuid(),
            Name = "User Test",
            Email = "usertest@gmail.com",
            Password = PasswordHasherTests.HashPassword("User1@Test-123"),
            Balance = (decimal)149.99
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
            Password = PasswordHasherTests.HashPassword("Updated@User-123"),
            Bio = "Now I have a bio",
            ProfilePicture = "https://i.pinimg.com/736x/d6/82/57/d682577ac42e84125461689aa9b4623a.jpg",
            Balance = (decimal)299.99
        };
    }

    #endregion
}
