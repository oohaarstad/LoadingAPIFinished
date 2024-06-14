using Microsoft.AspNetCore.Mvc;
using LoadingAPI.Data;
using LoadingAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LoadingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(AppDbContext context, ILogger<ReviewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("{sessionId}")]
        public async Task<IActionResult> SubmitReview(int sessionId, [FromBody] Review review)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid review model received.");
                return BadRequest(ModelState);
            }

            review.SessionId = sessionId;
            _context.Reviews.Add(review);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving review: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

            _logger.LogInformation($"Review submitted for session ID: {sessionId}, User ID: {review.UserId}");
            return CreatedAtAction(nameof(GetReview), new { id = review.ReviewId }, review);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                _logger.LogWarning($"Review with ID: {id} not found.");
                return NotFound();
            }
            return Ok(review);
        }
    }
}
