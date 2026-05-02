using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Order;
using T3awuny.Application.Helpers;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Specifications.OrderSpecs;

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
        public async Task<ActionResult<ApiResponse<OrderSummaryDto>>> CreateOrder(CreateOrderDto orderDto)
        {
            var buyerId = GetUserIdFromClaims();
            if(string.IsNullOrEmpty(buyerId))
                 return BadRequest(ApiResponse<OrderSummaryDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));

            var order = await _orderService.PlaceOrderAsync(buyerId,orderDto);
            if (!order.IsSuccess) 
                return BadRequest(order);

            return Ok(order);
        }

        [Authorize("TraderOnly")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<OrderSummaryDto>>>> GetMyOrders()
        {
            var buyerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(buyerId))
                return BadRequest(ApiResponse<IReadOnlyList<OrderSummaryDto>>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
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

        [Authorize("FarmerOnly")]
        [HttpPatch("{orderId}/confirm")]
        public async Task<ActionResult<ApiResponse<string>>> ConfirmOrder(int orderId)
        {
            var farmerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(farmerId))
                return BadRequest(ApiResponse<OrderResponseDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));

            var result = await _orderService.ConfirmOrderAsync(farmerId, orderId);
            if(!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }


        [Authorize("FarmerOnly")]
        [HttpPatch("{orderId}/reject")]
        public async Task<ActionResult<ApiResponse<string>>> RejectOrder(int orderId, string reason)
        {
            var farmerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(farmerId))
                return BadRequest(ApiResponse<OrderResponseDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));

            var result = await _orderService.RejectOrderAsync(farmerId, orderId,reason);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize("FarmerOrAdmin")]
        [HttpPatch("{orderId}/change-status")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateOrderStaus(int orderId, OrderStatus newStatus)
        {
            var userId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(userId))
                return BadRequest(ApiResponse<OrderResponseDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));

            var result = await _orderService.UpdateOrderStatusAsync(userId, orderId, newStatus);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("delivery-methods/{deliveryMethodId}")]
        public async Task<ActionResult<ApiResponse<DeliveryMethod>>> GetDeliveryMethod(int deliveryMethodId)
        {
            var result = await _orderService.GetDeliveryMethod(deliveryMethodId);
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize("FarmerOnly")]
        [HttpGet("my/farmer")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<OrderSummaryDto>>>> GetMyOrdersAsFarmer()
        {
            var farmerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(farmerId))
                return BadRequest(ApiResponse<IReadOnlyList<OrderSummaryDto>>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            var result = await _orderService.GetMyOrdersAsFarmerAsync(farmerId);
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize("AdminOnly")]
        [HttpGet("admin")]
        public async Task<ActionResult<ApiResponse<Pagination<OrderSummaryDto>>>> GetAllOrders([FromQuery] OrderSpecParams specs)
        {
            var result = await _orderService.GetAllOrdersAsync(specs);
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [Authorize("TraderOnly")]
        [HttpPatch("{orderId}/cancel")]
        public async Task<ActionResult<ApiResponse<string>>> CancelOrder(int orderId)
        {
            var buyerId = GetUserIdFromClaims();
            if (string.IsNullOrEmpty(buyerId))
                return BadRequest(ApiResponse<IReadOnlyList<OrderSummaryDto>>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            var result = await _orderService.CancelOrderAsync(buyerId, orderId);
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        private string GetUserIdFromClaims()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? string.Empty;
        }
    }
}
