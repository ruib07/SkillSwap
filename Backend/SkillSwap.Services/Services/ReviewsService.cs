using SkillSwap.Entities.Entities;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Repositories.Interfaces;

namespace SkillSwap.Services.Services;

public class ReviewsService
{
    private readonly IReviewsRepository _reviewsRepository;

    public ReviewsService(IReviewsRepository reviewsRepository)
    {
        _reviewsRepository = reviewsRepository;
    }

    public async Task<Reviews> GetReviewById(Guid id)
    {
        var review = await _reviewsRepository.GetReviewById(id);

        if (review == null) ErrorHelper.ThrowNotFoundException("Review not found.");

        return review;
    }

    public async Task<List<Reviews>> GetReviewBySessionId(Guid sessionId)
    {
        var reviewsBySession = await _reviewsRepository.GetReviewBySessionId(sessionId);

        if (reviewsBySession == null || reviewsBySession.Count == 0)
            ErrorHelper.ThrowNotFoundException("No reviews for that session.");

        return reviewsBySession;
    }

    public async Task<List<Reviews>> GetReviewsByReviewerId(Guid reviewerId)
    {
        var reviewsByReviewer = await _reviewsRepository.GetReviewsByReviewerId(reviewerId);

        if (reviewsByReviewer == null || reviewsByReviewer.Count == 0)
            ErrorHelper.ThrowNotFoundException("No reviews for that reviewer.");

        return reviewsByReviewer;
    }

    public async Task<Reviews> CreateReview(Reviews review)
    {
        if (review.Rating < 1 || review.Rating > 5) 
            ErrorHelper.ThrowBadRequestException("Rating must be between 1 and 5.");

        return await _reviewsRepository.CreateReview(review);
    }

    public async Task DeleteReview(Guid id)
    {
        await _reviewsRepository.DeleteReview(id);
    }
}
