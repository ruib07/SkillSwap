using Microsoft.AspNetCore.Mvc;
using Moq;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Controllers;
using SkillSwap.Services.Repositories.Interfaces;
using SkillSwap.Services.Services;
using static SkillSwap.Server.Models.Responses;

namespace SkillSwap.Tests.Controllers;

[TestFixture]
public class ReviewsControllerTests
{
    private Mock<IReviewsRepository> reviewsRepositoryMock;
    private ReviewsService reviewsService;
    private ReviewsController reviewsController;

    [SetUp]
    public void Setup()
    {
        reviewsRepositoryMock = new Mock<IReviewsRepository>();
        reviewsService = new ReviewsService(reviewsRepositoryMock.Object);
        reviewsController = new ReviewsController(reviewsService);
    }

    [Test]
    public async Task GetReviewById_ReturnsOkResult_WithReview()
    {
        var review = CreateReviewTemplate()[0];

        reviewsRepositoryMock.Setup(repo => repo.GetReviewById(review.Id)).ReturnsAsync(review);

        var result = await reviewsController.GetReviewById(review.Id);
        var okResult = result as OkObjectResult;
        var response = okResult.Value as Reviews;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response.SessionId, Is.EqualTo(review.SessionId));
            Assert.That(response.ReviewerId, Is.EqualTo(review.ReviewerId));
            Assert.That(response.Rating, Is.EqualTo(review.Rating));
            Assert.That(response.Comments, Is.EqualTo(review.Comments));
        });
    }

    [Test]
    public async Task GetReviewsByReviewerId_ReturnsOkResult_WithReview()
    {
        var review = CreateReviewTemplate();

        reviewsRepositoryMock.Setup(repo => repo.GetReviewBySessionId(review[0].SessionId)).ReturnsAsync(review);

        var result = await reviewsController.GetReviewBySessionId(review[0].SessionId);
        var okResult = result.Result as OkObjectResult;
        var response = okResult.Value as List<Reviews>;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response[0].SessionId, Is.EqualTo(review[0].SessionId));
            Assert.That(response[0].ReviewerId, Is.EqualTo(review[0].ReviewerId));
            Assert.That(response[0].Rating, Is.EqualTo(review[0].Rating));
            Assert.That(response[0].Comments, Is.EqualTo(review[0].Comments));
        });
    }

    [Test]
    public async Task GetReviewBySessionId_ReturnsOkResult_WithReview()
    {
        var review = CreateReviewTemplate();

        reviewsRepositoryMock.Setup(repo => repo.GetReviewsByReviewerId(review[0].ReviewerId)).ReturnsAsync(review);

        var result = await reviewsController.GetReviewsByReviewerId(review[0].ReviewerId);
        var okResult = result.Result as OkObjectResult;
        var response = okResult.Value as List<Reviews>;

        Assert.That(okResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(response[0].SessionId, Is.EqualTo(review[0].SessionId));
            Assert.That(response[0].ReviewerId, Is.EqualTo(review[0].ReviewerId));
            Assert.That(response[0].Rating, Is.EqualTo(review[0].Rating));
            Assert.That(response[0].Comments, Is.EqualTo(review[0].Comments));
        });
    }

    [Test]
    public async Task CreateReview_ReturnsCreatedResult_WithValidReview()
    {
        var newReview = CreateReviewTemplate()[0];

        reviewsRepositoryMock.Setup(repo => repo.CreateReview(It.IsAny<Reviews>())).ReturnsAsync(newReview);

        var result = await reviewsController.CreateReview(newReview);
        var createdResult = result.Result as ObjectResult;
        var response = createdResult.Value as CreationResponse;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(createdResult.StatusCode, Is.EqualTo(201));
            Assert.That(response.Message, Is.EqualTo("Review created successfully."));
            Assert.That(response.Id, Is.EqualTo(newReview.Id));
        });
    }

    [Test]
    public async Task DeleteReview_ReturnsNoContent_WhenReviewIsDeleted()
    {
        var review = CreateReviewTemplate()[0];

        reviewsRepositoryMock.Setup(repo => repo.GetReviewById(review.Id)).ReturnsAsync(review);
        reviewsRepositoryMock.Setup(repo => repo.DeleteReview(review.Id)).Returns(Task.CompletedTask);

        var result = await reviewsController.DeleteReview(review.Id);
        var noContentResult = result as NoContentResult;

        Assert.That(noContentResult.StatusCode, Is.EqualTo(204));
    }

    #region Private Methods

    private static List<Reviews> CreateReviewTemplate()
    {
        return new List<Reviews>()
        {
            new() {
                Id = Guid.NewGuid(),
                SessionId = Guid.NewGuid(),
                ReviewerId = Guid.NewGuid(),
                Rating = 5,
                Comments = "Comment test"
            }
        };
    }

    #endregion
}
