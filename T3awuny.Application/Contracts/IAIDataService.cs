
using T3awuny.Application.Common;
using T3awuny.Application.DTOs.AI_Dtos;

namespace T3awuny.Application.Contracts
{
    public interface IAIDataService
    {
        Task<IReadOnlyList<AIUserDto>> GetUsersData();
        Task<IReadOnlyList<AIReviewDto>> GetFarmerReviewsData();
        Task<IReadOnlyList<AIProductDto>> GetProductsData();
        Task<IReadOnlyList<AIOrderDto>> GetOrdersData();
        Task<IReadOnlyList<AIBidDto>> GetBidsData();
        Task<IReadOnlyList<AIAuctionDto>> GetAuctionsData();
    }
}
