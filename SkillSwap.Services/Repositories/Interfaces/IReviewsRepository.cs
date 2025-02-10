using SkillSwap.Entities.Entities;

namespace SkillSwap.Services.Repositories.Interfaces;

public interface IReviewsRepository
{
    Task<Reviews> GetReviewById(Guid id);
    Task<List<Reviews>> GetReviewBySessionId(Guid sessionId);
    Task<List<Reviews>> GetReviewsByReviewerId(Guid reviewerId);
    Task<Reviews> CreateReview(Reviews review);
    Task DeleteReview(Guid id);
}
