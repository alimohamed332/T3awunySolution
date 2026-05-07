using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;

namespace T3awuny.Application.Contracts
{
    public interface IAuctionService
    {
        // Farmer actions
        //Task<ApiResponse<AuctionResponseDto>> CreateAuctionAsync(
        //    string farmerId, CreateAuctionDto dto);
        //Task<ApiResponse<string>> CancelAuctionAsync(
        //    string farmerId, int auctionId);
        //Task<ApiResponse<IEnumerable<AuctionSummaryDto>>> GetMyAuctionsAsync(
        //    string farmerId);

        //// Public — buyers browse
        //Task<ApiResponse<AuctionResponseDto>> GetByIdAsync(int auctionId);
        //Task<ApiResponse<PagedResult<AuctionSummaryDto>>> GetAllAsync(
        //    AuctionFilterDto filter);

        //// Bidding
        //Task<ApiResponse<BidResponseDto>> PlaceBidAsync(
        //    string bidderId, int auctionId, PlaceBidDto dto);
        //Task<ApiResponse<IEnumerable<BidResponseDto>>> GetBidsAsync(
        //    int auctionId);
        //Task<ApiResponse<IEnumerable<AuctionSummaryDto>>> GetMyBidsAsync(
        //    string bidderId);

        //// System actions (called by background service)
        //Task ProcessAuctionStartsAsync();   // Scheduled → Active
        //Task ProcessAuctionEndsAsync();     // Active → Ended/Failed
    }
}
