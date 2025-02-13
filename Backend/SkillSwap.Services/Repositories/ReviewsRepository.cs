using Microsoft.EntityFrameworkCore;
using SkillSwap.Entities.Entities;
using SkillSwap.EntitiesConfiguration;
using SkillSwap.Services.Repositories.Interfaces;

namespace SkillSwap.Services.Repositories;

public class ReviewsRepository : IReviewsRepository
{
    private readonly SkillSwapDbContext _context;
    private DbSet<Reviews> Reviews => _context.Reviews;

    public ReviewsRepository(SkillSwapDbContext context)
    {
        _context = context;
    }

    public async Task<Reviews> GetReviewById(Guid id)
    {
        return await Reviews.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Reviews>> GetReviewBySessionId(Guid sessionId)
    {
        return await Reviews.AsNoTracking().Where(rb => rb.SessionId == sessionId).ToListAsync();
    }

    public async Task<List<Reviews>> GetReviewsByReviewerId(Guid reviewerId)
    {
        return await Reviews.AsNoTracking().Where(rb => rb.ReviewerId == reviewerId).ToListAsync();
    }

    public async Task<Reviews> CreateReview(Reviews review)
    {
        await Reviews.AddAsync(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task DeleteReview(Guid id)
    {
        var deleteReview = await GetReviewById(id);
        Reviews.Remove(deleteReview);
        await _context.SaveChangesAsync();
    }
}
