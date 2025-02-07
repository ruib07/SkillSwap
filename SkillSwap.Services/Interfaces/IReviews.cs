using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Interfaces;

public interface IReviews
{
    Task<Reviews> GetReviewById(Guid id);
    Task<Reviews> GetReviewBySessionId(Guid sessionId);
    Task<Reviews> GetReviewsByReviewerId(Guid reviewerId);
    Task<Reviews> CreateReview(Reviews review);
    Task DeleteReview(Guid id);
}
