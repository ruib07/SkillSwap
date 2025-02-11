using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Repositories;

namespace SkillSwap.Tests.Repositories;

[TestFixture]
public class ReviewsRepositoryTests
{
    private ReviewsRepository reviewsRepository;
    private SkillSwapDbContext context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SkillSwapDbContext>()
                     .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                     .Options;

        context = new SkillSwapDbContext(options);
        reviewsRepository = new ReviewsRepository(context);
    }

    [TearDown]
    public void TearDown()
    {
        context.Dispose();
    }

    [Test]
    public async Task GetReviewById_ReturnsReview()
    {
        var review = CreateReviewsTemplate()[0];
        await reviewsRepository.CreateReview(review);

        var result = await reviewsRepository.GetReviewById(review.Id);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(review.Id));
            Assert.That(result.SessionId, Is.EqualTo(review.SessionId));
            Assert.That(result.ReviewerId, Is.EqualTo(review.ReviewerId));
            Assert.That(result.Rating, Is.EqualTo(review.Rating));
            Assert.That(result.Comments, Is.EqualTo(review.Comments));
        });
    }

    [Test]
    public async Task GetReviewBySessionId_ReturnsReview()
    {
        var review = CreateReviewsTemplate();
        context.Reviews.AddRange(review);
        await context.SaveChangesAsync();

        var result = await reviewsRepository.GetReviewBySessionId(review[0].SessionId);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(review[0].Id));
            Assert.That(result[0].SessionId, Is.EqualTo(review[0].SessionId));
            Assert.That(result[0].ReviewerId, Is.EqualTo(review[0].ReviewerId));
            Assert.That(result[0].Rating, Is.EqualTo(review[0].Rating));
            Assert.That(result[0].Comments, Is.EqualTo(review[0].Comments));
        });
    }

    [Test]
    public async Task GetReviewsByReviewerId_ReturnsReview()
    {
        var review = CreateReviewsTemplate();
        context.Reviews.AddRange(review);
        await context.SaveChangesAsync();

        var result = await reviewsRepository.GetReviewsByReviewerId(review[0].ReviewerId);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(review[0].Id));
            Assert.That(result[0].SessionId, Is.EqualTo(review[0].SessionId));
            Assert.That(result[0].ReviewerId, Is.EqualTo(review[0].ReviewerId));
            Assert.That(result[0].Rating, Is.EqualTo(review[0].Rating));
            Assert.That(result[0].Comments, Is.EqualTo(review[0].Comments));
        });
    }

    [Test]
    public async Task CreateReview_AddsReview()
    {
        var newReview = CreateReviewsTemplate()[0];

        var result = await reviewsRepository.CreateReview(newReview);
        var addedReview = await reviewsRepository.GetReviewById(newReview.Id);

        Assert.That(addedReview, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(addedReview.Id, Is.EqualTo(newReview.Id));
            Assert.That(addedReview.SessionId, Is.EqualTo(newReview.SessionId));
            Assert.That(addedReview.ReviewerId, Is.EqualTo(newReview.ReviewerId));
            Assert.That(addedReview.Rating, Is.EqualTo(newReview.Rating));
            Assert.That(addedReview.Comments, Is.EqualTo(newReview.Comments));
        });
    }

    [Test]
    public async Task DeleteReview_DeletesSuccessfully()
    {
        var existingReview = CreateReviewsTemplate()[0];
        
        await reviewsRepository.CreateReview(existingReview);
        await reviewsRepository.DeleteReview(existingReview.Id);
        var retrievedNullReview = await reviewsRepository.GetReviewById(existingReview.Id);

        Assert.That(retrievedNullReview, Is.Null);
    }

    #region Private Methods

    private static List<Reviews> CreateReviewsTemplate()
    {
        return new List<Reviews>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                SessionId = Guid.NewGuid(),
                ReviewerId = Guid.NewGuid(),
                Rating = 4,
                Comments = "Review 1 Comment"
            },
            new()
            {
                Id = Guid.NewGuid(),
                SessionId = Guid.NewGuid(),
                ReviewerId = Guid.NewGuid(),
                Rating = 5,
                Comments = "Review 2 Comment"
            }
        };
    }

    #endregion
}
