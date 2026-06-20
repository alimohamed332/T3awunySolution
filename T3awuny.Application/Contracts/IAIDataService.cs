
using T3awuny.Application.Common;
using T3awuny.Application.DTOs.AI_Dtos;

namespace T3awuny.Application.Contracts
{
    public interface IAIDataService
    {
        Task<ApiResponse<IReadOnlyList<AIUserDto>>> GetUsersData();
        Task<ApiResponse<IReadOnlyList<AIReviewDto>>> GetFarmerReviewsData();
        Task<ApiResponse<IReadOnlyList<AIProductDto>>> GetProductsData();
        Task<ApiResponse<IReadOnlyList<AIOrderDto>>> GetOrdersData();
        Task<ApiResponse<IReadOnlyList<AIBidDto>>> GetBidsData();
        Task<ApiResponse<IReadOnlyList<AIAuctionDto>>> GetAuctionsData();
    }
}
