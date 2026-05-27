using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.DTOs.Review;

namespace T3awuny.Application.Contracts
{
    public interface IReviewService
    {
        // Buyer actions
        Task<ApiResponse<ReviewResponseDto>> CreateReviewAsync(string reviewerId, CreateReviewDto dto);

        // Public
        Task<ApiResponse<IReadOnlyList<ReviewResponseDto>>> GetUserReviewsAsync(string userId);
        Task<ApiResponse<UserRatingSummaryDto>> GetUserRatingSummaryAsync(string userId);

    //    // Admin
    //    Task<ApiResponse<string>> ApproveReviewAsync(int reviewId);
    //    Task<ApiResponse<string>> DeleteReviewAsync(int reviewId);
    //    Task<ApiResponse<IEnumerable<ReviewResponseDto>>> GetPendingReviewsAsync();
    }
}
