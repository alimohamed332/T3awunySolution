using T3awuny.Application.Common;
using T3awuny.Application.DTOs.Auction;
using T3awuny.Application.Helpers;
using T3awuny.Core.Entities.AuctionModule;
using T3awuny.Core.Specifications.AuctionSpecs;

namespace T3awuny.Application.Contracts
{
    public interface IAuctionService
    {
        // Farmer actions
        Task<ApiResponse<Auction>> CreateAuctionAsync(string farmerId, CreateAuctionDto dto);
        Task<ApiResponse<string>> CancelAuctionAsync(string farmerId, int auctionId); // Active → Canceled
        Task<ApiResponse<IReadOnlyList<AuctionSummaryDto>>> GetMyAuctionsAsync(string farmerId);

        // Public — buyers browse
        Task<ApiResponse<AuctionResponseDto>> GetByIdAsync(int auctionId);
        Task<ApiResponse<Pagination<AuctionSummaryDto>>> GetAllAsync(AuctionSpecParams filter);

        // Bidding
        Task<ApiResponse<BidResponseDto>> PlaceBidAsync(string bidderId, int auctionId, PlaceBidDto dto);
        //Task<ApiResponse<IEnumerable<BidResponseDto>>> GetBidsAsync(int auctionId);
        //Task<ApiResponse<IEnumerable<AuctionSummaryDto>>> GetMyBidsAsync(string bidderId);

        // System actions (called by background service)
        Task ProcessAuctionStartsAsync();   // Scheduled → Active
        Task ProcessAuctionEndsAsync();     // Active → Ended/Failed
    }
}
