using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Repositories;

namespace SkillSwap.Tests.Repositories;

[TestFixture]
public class SkillsRepositoryTests
{
    private SkillsRepository skillsRepository;
    private SkillSwapDbContext context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SkillSwapDbContext>()
                      .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                      .Options;

        context = new SkillSwapDbContext(options);
        skillsRepository = new SkillsRepository(context);
    }

    [TearDown]
    public void TearDown()
    {
        context.Dispose();
    }

    [Test]
    public async Task GetAllSkills_ReturnsListOfSkills()
    {
        var skillsList = CreateSkillsTemplate();
        context.Skills.AddRange(skillsList);
        await context.SaveChangesAsync();

        var result = await skillsRepository.GetAllSkills();

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result[0].Id, Is.EqualTo(skillsList[0].Id));
            Assert.That(result[0].Name, Is.EqualTo(skillsList[0].Name));
            Assert.That(result[0].Description, Is.EqualTo(skillsList[0].Description));
            Assert.That(result[1].Id, Is.EqualTo(skillsList[1].Id));
            Assert.That(result[1].Name, Is.EqualTo(skillsList[1].Name));
            Assert.That(result[1].Description, Is.EqualTo(skillsList[1].Description));
        });
    }

    [Test]
    public async Task GetSkillById_ReturnsSkill()
    {
        var skill = CreateSkillsTemplate()[0];
        context.Skills.Add(skill);
        await context.SaveChangesAsync();

        var result = await skillsRepository.GetSkillById(skill.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(skill.Id));
            Assert.That(result.Name, Is.EqualTo(skill.Name));
            Assert.That(result.Description, Is.EqualTo(skill.Description));
        });
    }

    [Test]
    public async Task CreateSkill_AddsSkill()
    {
        var newSkill = CreateSkillsTemplate()[0];

        var result = await skillsRepository.CreateSkill(newSkill);
        var addedSkill = await skillsRepository.GetSkillById(newSkill.Id);

        Assert.That(addedSkill, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(addedSkill.Id, Is.EqualTo(newSkill.Id));
            Assert.That(addedSkill.Name, Is.EqualTo(newSkill.Name));
            Assert.That(addedSkill.Description, Is.EqualTo(newSkill.Description));
        });
    }

    [Test]
    public async Task UpdateSkill_UpdatesSuccessfully()
    {
        var existingSkill = CreateSkillsTemplate()[0];
        await skillsRepository.CreateSkill(existingSkill);

        context.Entry(existingSkill).State = EntityState.Detached;

        var updatedSkill = UpdateSkillTemplate(id: existingSkill.Id);

        await skillsRepository.UpdateSkill(updatedSkill);
        var retrievedUpdatedSkill = await skillsRepository.GetSkillById(existingSkill.Id);

        Assert.That(retrievedUpdatedSkill, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(retrievedUpdatedSkill.Name, Is.EqualTo(updatedSkill.Name));
            Assert.That(retrievedUpdatedSkill.Description, Is.EqualTo(updatedSkill.Description));
        });
    }

    [Test]
    public async Task DeleteSkill_DeletesSuccessfully()
    {
        var existingSkill = CreateSkillsTemplate()[0];
        
        await skillsRepository.CreateSkill(existingSkill);
        await skillsRepository.DeleteSkill(existingSkill.Id);
        var retrievedNullSkill = await skillsRepository.GetSkillById(existingSkill.Id);

        Assert.That(retrievedNullSkill, Is.Null);
    }

    [Test]
    public async Task EnsureSkillNameIsUnique_ReturnsFalse_WhenNameIsUnique()
    {
        var result = await skillsRepository.EnsureSkillNameIsUnique("UniqueSkill");

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task EnsureSkillNameIsUnique_ReturnsTrue_WhenNameAlreadyExists()
    {
        var skillsList = CreateSkillsTemplate();
        context.Skills.AddRange(skillsList);
        await context.SaveChangesAsync();

        var result = await skillsRepository.EnsureSkillNameIsUnique("Skill1");

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task EnsureSkillNameIsUnique_ReturnsFalse_WhenUpdatingExistingSkill()
    {
        var existingSkill = CreateSkillsTemplate()[0];
        await skillsRepository.CreateSkill(existingSkill);

        var result = await skillsRepository.EnsureSkillNameIsUnique(existingSkill.Name, existingSkill.Id);

        Assert.That(result, Is.False);
    }

    #region Private Methods

    private static List<Skills> CreateSkillsTemplate()
    {
        return new List<Skills>()
        {
            new() { Id = Guid.NewGuid(), Name = "Skill1", Description = "Skill1 Description" },
            new() { Id = Guid.NewGuid(), Name = "Skill2", Description = "Skill2 Description" },
        };
    }

    private static Skills UpdateSkillTemplate(Guid id)
    {
        return new Skills()
        {
            Id = id,
            Name = "Skill1 Updated",
            Description = "Skill1 Description Updated"
        };
    }

    #endregion
}
