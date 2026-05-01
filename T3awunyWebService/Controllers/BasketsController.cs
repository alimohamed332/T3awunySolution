using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Core.Entities.BasketModule;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("TraderOnly")]
    public class BasketsController(IBaskeetService _basketService) : ControllerBase
    {
        [HttpGet("{basketId}")] // Get : /api/Basket/basketId
        public async Task<ActionResult<ApiResponse<CustomerBasket>>> GetBasket(string basketId)
        {
            var basket = await _basketService.GetBasketAsync(basketId);
            if (!basket.IsSuccess)
                return BadRequest(basket);
            return Ok(basket);
        }
        [HttpPost] // Post : /api/Basket
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdateBasket(CustomerBasket basket)
        {
            basket.Id = Guid.NewGuid().ToString();
            var result = await _basketService.CreateOrUpdateBasketAsync(basket);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpDelete("{basketId}")] // Delete : /api/Basket/basketID
        public async Task<ActionResult> DeleteBasket(string basketId)
        {
            var result = await _basketService.DeleteBasketAsync(basketId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
