using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Basket;
using T3awuny.Core.Entities.BasketModule;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("TraderOnly")]
    public class BasketsController(IBaskeetService _basketService) : ControllerBase
    {
        [Authorize("TraderOnly")]
        [HttpGet("{userId?}")] // Get : /api/Basket/basketId
        public async Task<ActionResult<ApiResponse<CustomerBasket>>> GetBasket(string? userId)
        {
            if (userId is null)
                userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? "";
            var basket = await _basketService.GetBasketAsync(userId);
            if (!basket.IsSuccess)
                return BadRequest(basket);
            return Ok(basket);
        }
        [Authorize("TraderOnly")]
        [HttpPost] // Post : /api/Basket
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdateBasket(CreateBasketDto basket)
        {
            //if(string.IsNullOrEmpty(basket.Id))
            //basket.Id = Guid.NewGuid().ToString();
            var traderId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? string.Empty; //will work as basket and user id at the same time
            basket.Id = traderId;
            var result = await _basketService.CreateOrUpdateBasketAsync(basket);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [Authorize("TraderOnly")]
        [HttpDelete("{basketId}")] // Delete : /api/Basket/basketID
        public async Task<ActionResult> DeleteBasket(string userId)
        {
            var result = await _basketService.DeleteBasketAsync(userId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
