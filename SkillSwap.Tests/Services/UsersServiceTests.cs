using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;
using System.Net;
using static SkillSwap.Server.Models.RecoverPassword;

namespace SkillSwap.Tests.Services;

[TestFixture]
public class UsersServiceTests
{
    private Mock<IUsersRepository> usersRepositoryMock;
    private UsersService usersService;

    [SetUp]
    public void Setup()
    {
        usersRepositoryMock = new Mock<IUsersRepository>();
        usersService = new UsersService(usersRepositoryMock.Object);
    }

    [Test]
    public async Task GetUserById_ReturnsUser()
    {
        var user = CreateUserTemplate();

        usersRepositoryMock.Setup(repo => repo.GetUserById(user.Id)).ReturnsAsync(user);

        var result = await usersService.GetUserById(user.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(user.Id));
            Assert.That(result.Name, Is.EqualTo(user.Name));
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(result.Password, Is.EqualTo(user.Password));
            Assert.That(result.Balance, Is.EqualTo(user.Balance));
        });
    }

    [Test]
    public void GetUserById_ReturnsNotFoundException_WhenUserNotFound()
    {
        usersRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<Guid>())).ReturnsAsync((Users)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await usersService.GetUserById(Guid.NewGuid()));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("User not found."));
        });
    }

    [Test]
    public async Task CreateUser_CreatesSuccessfully()
    {
        var user = CreateUserTemplate();

        usersRepositoryMock.Setup(repo => repo.CreateUser(user)).ReturnsAsync(user);

        var result = await usersService.CreateUser(user);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(user.Id));
            Assert.That(result.Name, Is.EqualTo(user.Name));
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(result.Password, Is.EqualTo(user.Password));
            Assert.That(result.Balance, Is.EqualTo(user.Balance));
        });
    }

    [Test]
    public void CreateUser_ReturnsConflictException_WhenEmailAlreadyExists()
    {
        var user = CreateUserTemplate();

        usersRepositoryMock.Setup(repo => repo.GetUserByEmail(user.Email)).ReturnsAsync(user);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await usersService.CreateUser(user));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(exception.Message, Is.EqualTo("Email already in use."));
        });
    }

    [Test]
    public void CreateUser_ReturnsBadRequestException_WhenNameEmailAndPasswordAreEmpty()
    {
        var invalidUser = InvalidCreateUserTemplate(string.Empty, string.Empty, string.Empty, 0);

        usersRepositoryMock.Setup(repo => repo.CreateUser(invalidUser)).ReturnsAsync(invalidUser);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await usersService.CreateUser(invalidUser));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(exception.Message, Is.EqualTo("Name, email, and password are required."));
        });
    }

    [Test]
    public void CreateUser_ReturnsBadRequestException_WhenBalanceIsNegative()
    {
        var invalidUser = InvalidCreateUserTemplate("User1", "user1@email.com", "User1@Test-123", -12);

        usersRepositoryMock.Setup(repo => repo.CreateUser(invalidUser)).ReturnsAsync(invalidUser);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await usersService.CreateUser(invalidUser));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(exception.Message, Is.EqualTo("Balance cannot be negative."));
        });
    }

    [Test]
    public async Task GeneratePasswordResetToken_ReturnsToken()
    {
        var user = CreateUserTemplate();

        usersRepositoryMock.Setup(repo => repo.CreateUser(user)).ReturnsAsync(user);
        usersRepositoryMock.Setup(repo => repo.GetUserByEmail(user.Email)).ReturnsAsync(user);
        usersRepositoryMock.Setup(repo => repo.GeneratePasswordResetToken(user.Id)).ReturnsAsync("token");

        var result = await usersService.GeneratePasswordResetToken(user.Email);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo("token"));
    }

    [Test]
    public async Task UpdatePassword_ValidToken_UpdatesPasswordSuccessfully()
    {
        var user = CreateUserTemplate();
        var updatePassword = UpdatePasswordTemplate("valid-token", "New@Password-123", "New@Password-123");
        var passwordResetToken = new PasswordResetToken { Token = updatePassword.Token, User = user };

        usersRepositoryMock.Setup(repo => repo.CreateUser(user)).ReturnsAsync(user);
        usersRepositoryMock.Setup(repo => repo.GetPasswordResetToken(updatePassword.Token)).ReturnsAsync(passwordResetToken);
        usersRepositoryMock.Setup(repo => repo.UpdateUser(It.IsAny<Users>())).Returns(Task.CompletedTask);
        usersRepositoryMock.Setup(repo => repo.RemovePasswordResetToken(passwordResetToken)).Returns(Task.CompletedTask);
        usersRepositoryMock.Setup(repo => repo.GetUserById(user.Id)).ReturnsAsync(user);

        await usersService.UpdatePassword(updatePassword.Token, updatePassword.NewPassword, updatePassword.ConfirmNewPassword);
        var result = await usersService.GetUserById(user.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Password, Is.EqualTo(user.Password));
    }

    [Test]
    public void UpdatePassword_EmptyPasswordFields_ThrowsBadRequestException()
    {
        var updatePassword = UpdatePasswordTemplate("valid-token", string.Empty, string.Empty);

        var exception = Assert.ThrowsAsync<CustomException>(() => usersService.UpdatePassword(updatePassword.Token, updatePassword.NewPassword, 
                                                                                                updatePassword.ConfirmNewPassword));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(exception.Message, Is.EqualTo("Password fields cannot be empty."));
        });
    }

    [Test]
    public void UpdatePassword_PasswordsDoNotMatch_ThrowsBadRequestException()
    {
        var updatePassword = UpdatePasswordTemplate("valid-token", "NewPassword123!", "DifferentPassword123!");

        var exception = Assert.ThrowsAsync<CustomException>(() => usersService.UpdatePassword(updatePassword.Token, updatePassword.NewPassword, 
                                                                                                updatePassword.ConfirmNewPassword));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(exception.Message, Is.EqualTo("Passwords do not match."));
        });
    }

    [Test]
    public void UpdatePassword_InvalidToken_ThrowsBadRequestException()
    {
        var updatePassword = UpdatePasswordTemplate("invalid-token", "NewPassword123!", "NewPassword123!");

        usersRepositoryMock.Setup(repo => repo.GetPasswordResetToken(updatePassword.Token)).ReturnsAsync((PasswordResetToken)null);

        var exception = Assert.ThrowsAsync<CustomException>(() => usersService.UpdatePassword(updatePassword.Token, updatePassword.NewPassword, 
                                                                                                updatePassword.ConfirmNewPassword));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(exception.Message, Is.EqualTo("Invalid or expired token."));
        });
    }

    [Test]
    public async Task UpdateUser_UpdatesSuccessfully()
    {
        var user = CreateUserTemplate();
        var updateUser = UpdateUserTemplate(user.Id, "userupdated@email.com");

        usersRepositoryMock.Setup(repo => repo.CreateUser(user)).ReturnsAsync(user);
        usersRepositoryMock.Setup(repo => repo.UpdateUser(updateUser)).Returns(Task.CompletedTask);
        usersRepositoryMock.Setup(repo => repo.GetUserById(user.Id)).ReturnsAsync(user);

        await usersService.UpdateUser(user.Id, updateUser);
        var result = await usersService.GetUserById(user.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(updateUser.Id));
            Assert.That(result.Name, Is.EqualTo(updateUser.Name));
            Assert.That(result.Email, Is.EqualTo(updateUser.Email));
            Assert.That(result.Balance, Is.EqualTo(updateUser.Balance));
        });
    }

    [Test]
    public void UpdateUser_ReturnsConflictException_WhenEmailAlreadyExists()
    {
        var userId = Guid.NewGuid();

        var existingUser = CreateUserTemplate();
        existingUser.Id = userId;
        existingUser.Email = "user1@email.com";

        var conflictingUser = new Users { Id = Guid.NewGuid(), Email = "userupdated@email.com" };
        var updateUser = UpdateUserTemplate(userId, "userupdated@email.com");

        usersRepositoryMock.Setup(repo => repo.GetUserById(userId)).ReturnsAsync(existingUser);
        usersRepositoryMock.Setup(repo => repo.GetUserByEmail(updateUser.Email)).ReturnsAsync(conflictingUser); 

        var exception = Assert.ThrowsAsync<CustomException>(() => usersService.UpdateUser(userId, updateUser));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            Assert.That(exception.Message, Is.EqualTo("Email already in use."));
        });
    }

    [Test]
    public async Task UpdateBalance_UpdatesSuccessfully()
    {
        var user = CreateUserTemplate();
        var newBalance = 199.99m;

        usersRepositoryMock.Setup(repo => repo.CreateUser(user)).ReturnsAsync(user);
        usersRepositoryMock.Setup(repo => repo.GetUserById(user.Id)).ReturnsAsync(user);
        usersRepositoryMock.Setup(repo => repo.UpdateBalance(user.Id, newBalance)).ReturnsAsync(newBalance);
        usersRepositoryMock.Setup(repo => repo.UpdateUser(It.IsAny<Users>())).Returns(Task.CompletedTask);

        await usersService.UpdateBalance(user.Id, newBalance);
        var result = await usersService.GetUserById(user.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Balance, Is.EqualTo(newBalance));
    }

    [Test]
    public void UpdateBalance_ReturnsBadRequest_WhenBalanceIsNegative()
    {
        var user = CreateUserTemplate();
        var newBalance = -199.99m;

        usersRepositoryMock.Setup(repo => repo.CreateUser(user)).ReturnsAsync(user);
        usersRepositoryMock.Setup(repo => repo.GetUserById(user.Id)).ReturnsAsync(user);

        var exception = Assert.ThrowsAsync<CustomException>(() => usersService.UpdateBalance(user.Id, newBalance));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(exception.Message, Is.EqualTo("Balance cannot be negative."));
        });
    }

    [Test]
    public async Task DeleteUser_DeletesSuccessfully()
    {
        var user = CreateUserTemplate();

        usersRepositoryMock.Setup(repo => repo.CreateUser(user)).ReturnsAsync(user);
        usersRepositoryMock.Setup(repo => repo.DeleteUser(user.Id)).Returns(Task.CompletedTask);
        usersRepositoryMock.Setup(repo => repo.GetUserById(user.Id)).ReturnsAsync((Users)null);

        await usersService.CreateUser(user);
        await usersService.DeleteUser(user.Id);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await usersService.GetUserById(user.Id));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("User not found."));
        });
    }

    #region Private Method

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

    private static Users InvalidCreateUserTemplate(string name, string email, string password, decimal balance)
    {
        return new Users()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Password = PasswordHasher.HashPassword(password),
            Balance = balance
        };
    }

    private static UpdatePasswordRequest UpdatePasswordTemplate(string token, string newPassword, string confirmNewPassword)
    {
        return new UpdatePasswordRequest()
        {
            Token = token,
            NewPassword = newPassword,
            ConfirmNewPassword = confirmNewPassword
        };
    }

    private static Users UpdateUserTemplate(Guid id, string email)
    {
        return new Users()
        {
            Id = id,
            Name = "User Updated",
            Email = email,
            Password = PasswordHasher.HashPassword("User@Updated-123"),
            Balance = 199.99m
        };
    }

    #endregion
}
