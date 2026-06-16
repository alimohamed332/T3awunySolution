using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Auction;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.Helpers;
using T3awuny.Core.Entities.AuctionModule;
using T3awuny.Core.Specifications.AuctionSpecs;
using T3awunyWebService.Hubs;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        private readonly IAuctionService _auctionService;
        private readonly IHubContext<AuctionHub> _hubContext;
        public AuctionsController(IAuctionService auctionService, IHubContext<AuctionHub> hubContext)
        {
            _auctionService = auctionService;
            _hubContext = hubContext;
        }
        [Authorize("FarmerOnly")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Auction>>> CreateAuction([FromBody] CreateAuctionDto dto)
        {
            var farmerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(farmerId))
                return BadRequest(ApiResponse<Auction>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            var auction = await _auctionService.CreateAuctionAsync(farmerId, dto);

            if (!auction.IsSuccess)
                return BadRequest(auction);
            return Ok(auction);
        }
        /// <summary>
        /// Get all auctions , sort possible values => (createdAt(default) , currentPrice (the highest price for the auction) , endDate , startDate)
        /// And Status values =>  Scheduled = 0 ,Active = 1, Ended = 2, Cancelled = 3,Failed = 4
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<Pagination<AuctionResponseWithNoBidsDto>>>> GetAll([FromQuery] AuctionSpecParams filter)
        {
            var auctions = await _auctionService.GetAllAsync(filter);
            if (!auctions.IsSuccess)
                return NotFound(auctions);
            return Ok(auctions);
        }
        /// <summary>
        /// Get acution details by auction id
        /// </summary>
        /// <param name="auctionId"></param>
        /// <returns></returns>
        [HttpGet("{auctionId}")]
        public async Task<ActionResult<ApiResponse<AuctionResponseDto>>> GetAuctionById(int auctionId)
        {
            var auction = await _auctionService.GetByIdAsync(auctionId);
            if (!auction.IsSuccess)
                return NotFound(auction);
            return Ok(auction);
        }
        /// <summary>
        /// Get acution details by product id which the acution created on
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet("products/{productId}")]
        public async Task<ActionResult<ApiResponse<AuctionResponseDto>>> GetAuctionByProductId(int productId)
        {
            var auction = await _auctionService.GetByProductIdAsync(productId);
            if (!auction.IsSuccess)
                return NotFound(auction);
            return BadRequest(auction);
        }

        [Authorize("FarmerOnly")]
        [HttpGet("my")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<AuctionResponseWithNoBidsDto>>>> GetMyAuctions()
        {
            var farmerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(farmerId))
                return BadRequest(ApiResponse<Auction>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            var auctions = await _auctionService.GetMyAuctionsAsync(farmerId);
            if (!auctions.IsSuccess)
                return NotFound(auctions);
            return Ok(auctions);
        }

        [Authorize("FarmerOrAdmin")]
        [HttpDelete("{auctionId}")]
        public async Task<ActionResult<ApiResponse<string>>> CancelAcution(int auctionId)
        {
            var farmerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(farmerId))
                return BadRequest(ApiResponse<Auction>.Fail("معرف المستخدم غير موجود في الرمز المميز"));

            var result = await _auctionService.CancelAuctionAsync(farmerId, auctionId);
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize("TraderOnly")]
        [HttpPost("{auctionId}/bids")]
        public async Task<ActionResult<ApiResponse<BidResponseDto>>> CreateBid(int auctionId, [FromBody] PlaceBidDto dto)
        {
            var traderId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(traderId))
                return BadRequest(ApiResponse<Auction>.Fail("معرف المستخدم غير موجود في الرمز المميز"));

            var result = await _auctionService.PlaceBidAsync(traderId,auctionId, dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            // Broadcast to all watchers via SignalR
            await _hubContext.Clients.Group($"auction_{auctionId}").SendAsync("bidplaced"
                , new
                {
                    AuctionId = auctionId,
                    //BidderName = bid.Bidder?.FullName,
                    BidderId = traderId,
                    dto.Amount,
                    CurrentPrice = dto.Amount,
                    BidTime = result.Data?.BidTime ?? DateTime.UtcNow
                });
            return Ok(result);
        }
        /// <summary>
        /// Get all auction bids by acution id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/bids")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<BidResponseDto>>>> GetAuctionBids(int id)
        {
            var result = await _auctionService.GetBidsAsync(id);
            if(!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        [Authorize("TraderOnly")]
        [HttpGet("my/bids")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<BidResponseDto>>>> GetMyBidsAsTrader()
        {
            var traderId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(traderId))
                return BadRequest(ApiResponse<Auction>.Fail("معرف المستخدم غير موجود في الرمز المميز"));

            var result = await _auctionService.GetMyBidsAsync(traderId);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }

        [Authorize("TraderOnly")]
        [HttpGet("my-winning-auctions")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<MyWinningtAuctionsDto>>>> GetMyWinningtAuctions()
        {
            var traderId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(traderId))
                return BadRequest(ApiResponse<IReadOnlyList<MyWinningtAuctionsDto>>.Fail("معرف المستخدم غير موجود في الرمز المميز"));

            var result = await _auctionService.GetMyWinningtAuctions(traderId);
            if (!result.IsSuccess)
                return NotFound(result);
            return Ok(result);
        }
        private string GetUserIdFromClaims()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? string.Empty;
        }
    }
}
