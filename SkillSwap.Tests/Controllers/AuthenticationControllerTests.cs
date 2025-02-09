using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Server.Controllers;
using SkillSwap.Server.Models;
using SkillSwap.Tests.Helpers;
using System.Security.Cryptography;

namespace SkillSwap.Tests.Controllers;

[TestFixture]
public class AuthenticationControllerTests
{
    private SkillSwapDbContext context;
    private JwtSettings jwt;
    private AuthenticationController authentication;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SkillSwapDbContext>()
                      .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                      .Options;

        context = new SkillSwapDbContext(options);
        var key = GenerateRandomKey();
        jwt = new JwtSettings()
        {
            Issuer = "testIssuer",
            Audience = "testAudience",
            Key = key,
        };
        authentication = new AuthenticationController(context, jwt);
    }

    [TearDown]
    public void TearDown()
    {
        context.Dispose();
    }

    [Test]
    public async Task Login_ReturnsOk_WhenCredentialsAreValid()
    {
        var user = new Users()
        {
            Name = "Valid User",
            Email = "validuser@gmail.com",
            Password = PasswordHasherTests.HashPassword("Valid@UserPassword-123"),
            Balance = 150
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var loginRequest = new UserAuthentication.LoginRequest()
        {
            Email = user.Email,
            Password = "Valid@UserPassword-123",
        };

        var result = await authentication.Login(loginRequest);
        var okResult = result as OkObjectResult;
        var loginResponse = okResult.Value as UserAuthentication.LoginResponse;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.Value, Is.Not.Null);
        Assert.That(loginResponse, Is.Not.Null);
    }

    [Test]
    public async Task Login_ReturnsUnauthorized_WhenUserNotFound()
    {
        var loginRequest = new UserAuthentication.LoginRequest()
        {
            Email = "invaliduser@gmail.com",
            Password = "Invalid@UserPassword-123",
        };

        var result = await authentication.Login(loginRequest);
        var unauthorizedResult = result as UnauthorizedObjectResult;

        Assert.That(unauthorizedResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(unauthorizedResult.StatusCode, Is.EqualTo(401));
            Assert.That(unauthorizedResult.Value, Is.EqualTo("User not found."));
        });
    }

    [Test]
    public async Task Login_ReturnsBadRequest_WhenLoginRequestIsNull()
    {
        var result = await authentication.Login(null);
        var badRequestResult = result as BadRequestObjectResult;

        Assert.That(badRequestResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Email and password are mandatory."));
        });
    }

    [Test]
    public async Task Login_ReturnsBadRequest_WhenEmailIsNull()
    {
        var loginRequest = new UserAuthentication.LoginRequest()
        {
            Email = "",
            Password = "Invalid@UserPassword-123",
        };

        var result = await authentication.Login(loginRequest);
        var badRequestResult = result as BadRequestObjectResult;

        Assert.That(badRequestResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Email is required."));
        });
    }

    [Test]
    public async Task Login_ReturnsBadRequest_WhenPasswordlIsNull()
    {
        var loginRequest = new UserAuthentication.LoginRequest()
        {
            Email = "invaliduser@gmail.com",
            Password = "",
        };

        var result = await authentication.Login(loginRequest);
        var badRequestResult = result as BadRequestObjectResult;

        Assert.That(badRequestResult, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
            Assert.That(badRequestResult.Value, Is.EqualTo("Password is required."));
        });
    }

    #region Private Methods

    private static string GenerateRandomKey()
    {
        byte[] keyBytes = new byte[32];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(keyBytes);
        }
        string base64Key = Convert.ToBase64String(keyBytes);

        base64Key = base64Key.Replace('_', '/').Replace('-', '+');

        return base64Key;
    }

    #endregion
}
