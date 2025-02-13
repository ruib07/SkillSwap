using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;
using System.Net;

namespace SkillSwap.Tests.Services;

[TestFixture]
public class ReviewsServiceTests
{
    private Mock<IReviewsRepository> reviewsRepositoryMock;
    private ReviewsService reviewsService;

    [SetUp]
    public void Setup()
    {
        reviewsRepositoryMock = new Mock<IReviewsRepository>();
        reviewsService = new ReviewsService(reviewsRepositoryMock.Object);
    }

    [Test]
    public async Task GetReviewById_ReturnsReview()
    {
        var review = CreateReviewTemplate()[0];

        reviewsRepositoryMock.Setup(repo => repo.GetReviewById(review.Id)).ReturnsAsync(review);

        var result = await reviewsService.GetReviewById(review.Id);

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
    public void GetReviewById_ReturnsNotFoundException_WhenReviewIdDontExist()
    {
        reviewsRepositoryMock.Setup(repo => repo.GetReviewById(It.IsAny<Guid>())).ReturnsAsync((Reviews)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await reviewsService.GetReviewById(Guid.NewGuid()));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("Review not found."));
        });
    }

    [Test]
    public async Task GetReviewBySessionId_ReturnsReviews()
    {
        var reviews = CreateReviewTemplate();

        reviewsRepositoryMock.Setup(repo => repo.GetReviewBySessionId(reviews[0].SessionId)).ReturnsAsync(reviews);

        var result = await reviewsService.GetReviewBySessionId(reviews[0].SessionId);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(reviews[0].Id));
            Assert.That(result[0].SessionId, Is.EqualTo(reviews[0].SessionId));
            Assert.That(result[0].ReviewerId, Is.EqualTo(reviews[0].ReviewerId));
            Assert.That(result[0].Rating, Is.EqualTo(reviews[0].Rating));
            Assert.That(result[0].Comments, Is.EqualTo(reviews[0].Comments));
        });
    }

    [Test]
    public void GetReviewBySessionId_ReturnsNotFoundException_WhenSessionIdDontExist()
    {
        reviewsRepositoryMock.Setup(repo => repo.GetReviewBySessionId(It.IsAny<Guid>())).ReturnsAsync((List<Reviews>)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await reviewsService.GetReviewBySessionId(Guid.NewGuid()));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("No reviews for that session."));
        });
    }

    [Test]
    public async Task GetReviewsByReviewerId_ReturnsReviews()
    {
        var reviews = CreateReviewTemplate();

        reviewsRepositoryMock.Setup(repo => repo.GetReviewsByReviewerId(reviews[0].ReviewerId)).ReturnsAsync(reviews);

        var result = await reviewsService.GetReviewsByReviewerId(reviews[0].ReviewerId);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(reviews[0].Id));
            Assert.That(result[0].SessionId, Is.EqualTo(reviews[0].SessionId));
            Assert.That(result[0].ReviewerId, Is.EqualTo(reviews[0].ReviewerId));
            Assert.That(result[0].Rating, Is.EqualTo(reviews[0].Rating));
            Assert.That(result[0].Comments, Is.EqualTo(reviews[0].Comments));
        });
    }

    [Test]
    public void GetReviewsByReviewerId_ReturnsNotFoundException_WhenReviewerIdDontExist()
    {
        reviewsRepositoryMock.Setup(repo => repo.GetReviewsByReviewerId(It.IsAny<Guid>())).ReturnsAsync((List<Reviews>)null);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await reviewsService.GetReviewsByReviewerId(Guid.NewGuid()));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("No reviews for that reviewer."));
        });
    }

    [Test]
    public async Task CreateReview_CreatesSuccessfully()
    {
        var review = CreateReviewTemplate()[0];

        reviewsRepositoryMock.Setup(repo => repo.CreateReview(review)).ReturnsAsync(review);

        var result = await reviewsService.CreateReview(review);

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
    public void CreateReview_ReturnsBadRequestException_WhenRatingIsInvalid()
    {
        var review = CreateInvalidReviewTemplate();

        reviewsRepositoryMock.Setup(repo => repo.CreateReview(review)).ReturnsAsync(review);

        var exception = Assert.ThrowsAsync<CustomException>(async () => await reviewsService.CreateReview(review));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(exception.Message, Is.EqualTo("Rating must be between 1 and 5."));
        });
    }

    [Test]
    public async Task DeleteReview_DeletesSuccessfully()
    {
        var review = CreateReviewTemplate()[0];

        reviewsRepositoryMock.Setup(repo => repo.CreateReview(review)).ReturnsAsync(review);
        reviewsRepositoryMock.Setup(repo => repo.DeleteReview(review.Id)).Returns(Task.CompletedTask);
        reviewsRepositoryMock.Setup(repo => repo.GetReviewById(review.Id)).ReturnsAsync((Reviews)null);

        await reviewsService.CreateReview(review);
        await reviewsService.DeleteReview(review.Id);
        var exception = Assert.ThrowsAsync<CustomException>(async () => await reviewsService.GetReviewById(review.Id));

        Assert.Multiple(() =>
        {
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(exception.Message, Is.EqualTo("Review not found."));
        });
    }

    #region Private Methods

    private static List<Reviews> CreateReviewTemplate()
    {
        return new List<Reviews>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                SessionId = Guid.NewGuid(),
                ReviewerId = Guid.NewGuid(),
                Rating = 5,
                Comments = "Great session!"
            }
        };
    }

    private static Reviews CreateInvalidReviewTemplate()
    {
        return new Reviews()
        {
            Id = Guid.NewGuid(),
            SessionId = Guid.NewGuid(),
            ReviewerId = Guid.NewGuid(),
            Rating = 6,
            Comments = "Great session!"
        };
    }

    #endregion
}
