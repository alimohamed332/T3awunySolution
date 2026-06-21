using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.AI_Dtos;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IAIDataService _aiService;

        public AIController(IAIDataService aiService)
        {
            _aiService = aiService;
        }
        [HttpGet("users")]
        public async Task<ActionResult<IReadOnlyList<AIUserDto>>> GetAllUsers()
        {
            var result = await _aiService.GetUsersData();
            return Ok(result);
        }

        [HttpGet("reviews")]
        public async Task<ActionResult<IReadOnlyList<AIReviewDto>>> GetAllReviews()
        {
            var result = await _aiService.GetFarmerReviewsData();;
            return Ok(result);
        }

        [HttpGet("products")]
        public async Task<ActionResult<IReadOnlyList<AIProductDto>>> GetAllProducts()
        {
            var result = await _aiService.GetProductsData();
            return Ok(result);
        }

        [HttpGet("orders")]
        public async Task<ActionResult<IReadOnlyList<AIOrderDto>>> GetAllOrders()
        {
            var result = await _aiService.GetOrdersData();
            return Ok(result);
        }

        [HttpGet("bids")]
        public async Task<ActionResult<IReadOnlyList<AIBidDto>>> GetAllBids()
        {
            var result = await _aiService.GetBidsData();
            return Ok(result);
        }

        [HttpGet("auctions")]
        public async Task<ActionResult<IReadOnlyList<AIAuctionDto>>> GetAllAuctions()
        {
            var result = await _aiService.GetAuctionsData();
            return Ok(result);
        }


    }
}
