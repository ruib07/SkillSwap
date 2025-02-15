using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories;

namespace SkillSwap.Tests.Repositories;

[TestFixture]
public class UsersRepositoryTests
{
    private UsersRepository usersRepository;
    private SkillSwapDbContext context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SkillSwapDbContext>()
                      .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                      .Options;

        context = new SkillSwapDbContext(options);
        usersRepository = new UsersRepository(context);
    }

    [TearDown]
    public void TearDown()
    {
        context.Dispose();
    }

    [Test]
    public async Task GetUserById_ReturnsUser()
    {
        var user = CreateUserTemplate()[0];

        await usersRepository.CreateUser(user);

        var result = await usersRepository.GetUserById(user.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(user.Id));
            Assert.That(result.Name, Is.EqualTo(user.Name));
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(result.Balance, Is.EqualTo(user.Balance));
        });
    }

    [Test]
    public async Task GetUserByEmail_ReturnsUser()
    {
        var user = CreateUserTemplate()[0];

        await usersRepository.CreateUser(user);

        var result = await usersRepository.GetUserByEmail(user.Email);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(user.Id));
            Assert.That(result.Name, Is.EqualTo(user.Name));
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(result.Balance, Is.EqualTo(user.Balance));
        });
    }

    [Test]
    public async Task GetMentors_ReturnsListOfMentors()
    {
        var mentors = CreateUserTemplate();
        context.Users.AddRange(mentors);
        await context.SaveChangesAsync();

        var result = await usersRepository.GetMentors();

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result[0].Id, Is.EqualTo(mentors[0].Id));
            Assert.That(result[0].Name, Is.EqualTo(mentors[0].Name));
            Assert.That(result[0].Email, Is.EqualTo(mentors[0].Email));
            Assert.That(result[0].Balance, Is.EqualTo(mentors[0].Balance));
            Assert.That(result[0].IsMentor, Is.EqualTo(mentors[0].IsMentor));
        });
    }

    [Test]
    public async Task CreateUser_AddsUser()
    {
        var newUser = CreateUserTemplate()[0];

        var result = await usersRepository.CreateUser(newUser);
        var addedUser = await context.Users.FindAsync(newUser.Id);

        Assert.That(addedUser, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(addedUser.Id, Is.EqualTo(newUser.Id));
            Assert.That(addedUser.Name, Is.EqualTo(newUser.Name));
            Assert.That(addedUser.Email, Is.EqualTo(newUser.Email));
            Assert.That(addedUser.Balance, Is.EqualTo(newUser.Balance));
        });
    }

    [Test]
    public async Task GeneratePasswordResetToken_CreatesTokenSuccessfully()
    {
        var existingUser = CreateUserTemplate()[0];

        await usersRepository.CreateUser(existingUser);
        var token = await usersRepository.GeneratePasswordResetToken(existingUser.Id);

        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.Not.Empty);
    }

    [Test]
    public async Task GetPasswordResetToken_ReturnsToken()
    {
        var existingUser = CreateUserTemplate()[0];

        await usersRepository.CreateUser(existingUser);

        var token = await usersRepository.GeneratePasswordResetToken(existingUser.Id);
        var savedToken = await usersRepository.GetPasswordResetToken(token);

        Assert.Multiple(() =>
        {
            Assert.That(savedToken.UserId, Is.EqualTo(existingUser.Id));
            Assert.That(savedToken.Token, Is.EqualTo(token));
            Assert.That(savedToken.ExpiryDate, Is.GreaterThan(DateTime.UtcNow));
        });
    }

    [Test]
    public async Task RemovePasswordResetToken_RemovesToken()
    {
        var existingUser = CreateUserTemplate()[0];

        await usersRepository.CreateUser(existingUser);

        var token = await usersRepository.GeneratePasswordResetToken(existingUser.Id);
        var savedToken = await usersRepository.GetPasswordResetToken(token);
        await usersRepository.RemovePasswordResetToken(savedToken);
        var deletedToken = await usersRepository.GetPasswordResetToken(token);

        Assert.Multiple(() =>
        {
            Assert.That(savedToken, Is.Not.Null);
            Assert.That(deletedToken, Is.Null);
        });
    }

    [Test]
    public async Task UpdateUser_UpdatesSuccessfully()
    {
        var existingUser = CreateUserTemplate()[0];
        await usersRepository.CreateUser(existingUser);

        context.Entry(existingUser).State = EntityState.Detached;

        var updatedUser = UpdateUserTemplate(id: existingUser.Id);

        await usersRepository.UpdateUser(updatedUser);
        var retrievedUpdatedUser = await usersRepository.GetUserById(existingUser.Id);

        Assert.That(retrievedUpdatedUser, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(retrievedUpdatedUser.Name, Is.EqualTo(updatedUser.Name));
            Assert.That(retrievedUpdatedUser.Email, Is.EqualTo(updatedUser.Email));
            Assert.That(retrievedUpdatedUser.Balance, Is.EqualTo(updatedUser.Balance));
        });
    }

    [Test]
    public async Task UpdateBalance_UpdatesSuccessfully()
    {
        var existingUser = CreateUserTemplate()[0];
        var newBalanceValue = 199.99m;

        await usersRepository.CreateUser(existingUser);
        await usersRepository.UpdateBalance(existingUser, newBalanceValue);
        var retrievedUser = await context.Users.FindAsync(existingUser.Id);

        Assert.That(retrievedUser, Is.Not.Null);
        Assert.That(retrievedUser.Balance, Is.EqualTo(newBalanceValue));
    }

    [Test]
    public async Task DeleteUser_DeletesSuccessfully()
    {
        var existingUser = CreateUserTemplate()[0];

        await usersRepository.CreateUser(existingUser);
        await usersRepository.DeleteUser(existingUser.Id);
        var retrievedNullUser = await context.Users.FindAsync(existingUser.Id);

        Assert.That(retrievedNullUser, Is.Null);
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
                Password = PasswordHasher.HashPassword("User@Test-123"),
                Balance = 149.99m,
                IsMentor = true
            }
        };
    }

    private static Users UpdateUserTemplate(Guid id)
    {
        return new Users()
        {
            Id = id,
            Name = "User Updated",
            Email = "userupdate@gmail.com",
            Password = PasswordHasher.HashPassword("User@Updated-123"),
            Balance = 199.99m,
            IsMentor = false
        };
    }

    #endregion
}

