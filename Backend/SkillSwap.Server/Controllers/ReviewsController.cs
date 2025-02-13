using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillSwap.Entities.Entities;
using SkillSwap.Server.Constants;
using SkillSwap.Services.Services;
using static SkillSwap.Server.Models.Responses;

namespace SkillSwap.Server.Controllers;

[Route("reviews")]
public class ReviewsController : ControllerBase
{
    private readonly ReviewsService _reviews;

    public ReviewsController(ReviewsService reviews)
    {
        _reviews = reviews;
    }

    // GET reviews/{id}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReviewById(Guid id)
    {
        var review = await _reviews.GetReviewById(id);

        return Ok(review);
    }

    // GET reviews/bysession/{sessionId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("bysession/{sessionId}")]
    public async Task<ActionResult<List<Reviews>>> GetReviewBySessionId(Guid sessionId)
    {
        var reviewsBySession = await _reviews.GetReviewBySessionId(sessionId);

        return Ok(reviewsBySession);
    }

    // GET reviews/byreviewer/{reviewerId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpGet("byreviewer/{reviewerId}")]
    public async Task<ActionResult<List<Reviews>>> GetReviewsByReviewerId(Guid reviewerId)
    {
        var reviewsByReviewer = await _reviews.GetReviewsByReviewerId(reviewerId);

        return Ok(reviewsByReviewer);
    }

    // POST reviews
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpPost]
    public async Task<ActionResult<Reviews>> CreateReview([FromBody] Reviews review)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        await _reviews.CreateReview(review);

        return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, new CreationResponse()
        {
            Message = "Review created successfully.",
            Id = review.Id
        });
    }

    // DELETE reviews/{reviewId}
    [Authorize(Policy = ApiConstants.PolicyUser)]
    [HttpDelete("{reviewId}")]
    public async Task<IActionResult> DeleteReview(Guid reviewId)
    {
        await _reviews.DeleteReview(reviewId);

        return NoContent();
    }
}
