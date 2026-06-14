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
        /// <summary>
        /// Admins only
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        [Authorize("AdminOnly")]
        [HttpPost("{reviewId}/approve")]
        public async Task<ActionResult<ApiResponse<string>>> ApproveReview(int reviewId)
        {
            var result = await _reviewService.ApproveReviewAsync(reviewId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Allowed for admins to delete inappropriate reviews. This action will permanently remove the review from the system.
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        [Authorize("AdminOnly")]
        [HttpDelete("{reviewId}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteReview(int reviewId)
        {
            var result = await _reviewService.DeleteReviewAsync(reviewId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Admins only 
        /// </summary>
        /// <returns></returns>
        [Authorize("AdminOnly")]
        [HttpGet("pending")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<ReviewResponseDto>>>> GetPendingReviews()
        {
            var result = await _reviewService.GetPendingReviewsAsync();
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
    }
}
