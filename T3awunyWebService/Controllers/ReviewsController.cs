using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Review;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        [Authorize("FarmerOrTrader")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ReviewResponseDto>>> CreateReview([FromBody] CreateReviewDto dto)
        {
            var reviewerId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(reviewerId))
                return Unauthorized(ApiResponse<ReviewResponseDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));

            var result = await _reviewService.CreateReviewAsync(reviewerId, dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("users/{userId}")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<ReviewResponseDto>>>> GetUserReviews(string userId)
        {
            var result = await _reviewService.GetUserReviewsAsync(userId);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        [HttpGet("users/{userId}/rating-summary")]
        public async Task<ActionResult<ApiResponse<UserRatingSummaryDto>>> GetUserRatingSummary(string userId)
        {
            var result = await _reviewService.GetUserRatingSummaryAsync(userId);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
    }
}
