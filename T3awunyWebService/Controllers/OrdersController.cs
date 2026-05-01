using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Order;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [Authorize("TraderOnly")]
        [HttpPost] //  Post: /api/orders
        public async Task<ActionResult<ApiResponse<OrderSummeryDto>>> CreateOrder(CreateOrderDto orderDto)
        {
            var buyerId = GetUserIdFromClaims();
            if(string.IsNullOrEmpty(buyerId))
                 return BadRequest(ApiResponse<OrderSummeryDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));

            var order = await _orderService.PlaceOrderAsync(buyerId,orderDto);
            if (!order.IsSuccess) 
                return BadRequest(order);

            return Ok(order);
        }

        [Authorize("TraderOnly")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<OrderSummeryDto>>>> GetMyOrders()
        {
            var buyerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(buyerId))
                return BadRequest(ApiResponse<IReadOnlyList<OrderSummeryDto>>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            var orders = await _orderService.GetOrdersForBuyerAsync(buyerId);
            if (!orders.IsSuccess)
                return BadRequest(orders);
            return Ok(orders);
        }


        [Authorize("TraderOnly")]
        [HttpGet("{orderId}")]
        public async Task<ActionResult<ApiResponse<OrderResponseDto>>> GetOrder(int orderId)
        {
            var buyerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(buyerId))
                return BadRequest(ApiResponse<OrderResponseDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));

            var order = await _orderService.GetOrderDetailsAsync(buyerId,orderId);
            if (!order.IsSuccess)
                return BadRequest(order);
            return Ok(order);
        }
        private string GetUserIdFromClaims()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? string.Empty;
        }
    }
}
