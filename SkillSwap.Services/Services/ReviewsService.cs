using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Helpers;
using SkillSwap.Services.Interfaces;

namespace SkillSwap.Services.Services;

public class ReviewsService : IReviews
{
    private readonly SkillSwapDbContext _context;

    public ReviewsService(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<Reviews> GetReviewById(Guid id)
    {
        var review = await _context.Reviews.AsNoTracking()
                     .FirstOrDefaultAsync(r => r.Id == id);

        if (review == null) ErrorHelper.ThrowNotFoundException("Review not found.");

        return review;
    }

    public async Task<List<Reviews>> GetReviewBySessionId(Guid sessionId)
    {
        var reviewsBySession = await _context.Reviews.AsNoTracking()
                               .Where(rb => rb.SessionId == sessionId)
                               .ToListAsync();

        if (reviewsBySession == null || reviewsBySession.Count == 0)
            ErrorHelper.ThrowNotFoundException("No reviews for that session.");

        return reviewsBySession;
    }

    public async Task<List<Reviews>> GetReviewsByReviewerId(Guid reviewerId)
    {
        var reviewsByReviewer = await _context.Reviews.AsNoTracking()
                               .Where(rb => rb.ReviewerId == reviewerId)
                               .ToListAsync();

        if (reviewsByReviewer == null || reviewsByReviewer.Count == 0)
            ErrorHelper.ThrowNotFoundException("No reviews for that reviewer.");

        return reviewsByReviewer;
    }

    public async Task<Reviews> CreateReview(Reviews review)
    {
        if (review.Rating < 1 || review.Rating > 5) ErrorHelper.ThrowBadRequestException("Rating must be between 1 and 5.");

        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();

        return review;
    }

    public async Task DeleteReview(Guid id)
    {
        var deleteReview = await _context.Reviews.FirstOrDefaultAsync(dr => dr.Id == id);

        if (deleteReview == null) ErrorHelper.ThrowNotFoundException("Review not found.");

        _context.Reviews.Remove(deleteReview);
        await _context.SaveChangesAsync();
    }
}
