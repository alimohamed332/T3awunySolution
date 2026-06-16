using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.DTOs.Order;
using T3awuny.Application.Helpers;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Specifications.OrderSpecs;

namespace T3awuny.Application.Contracts
{
    public interface IOrderService
    {
        // Buyer actions
        Task<ApiResponse<OrderSummaryDto>> PlaceOrderAsync(string buyerId, CreateOrderDto dto);
        Task<ApiResponse<string>> CancelOrderAsync(string buyerId, int orderId);
        Task<ApiResponse<IReadOnlyList<OrderSummaryDto>>> GetOrdersForBuyerAsync(string buyerId);
        Task<ApiResponse<OrderResponseDto>> GetOrderDetailsAsync(string userId, int orderId);

        //// Farmer actions
        Task<ApiResponse<string>> ConfirmOrderAsync(string farmerId, int orderId);
        Task<ApiResponse<string>> RejectOrderAsync(string farmerId, int orderId, string reason);
        Task<ApiResponse<string>> UpdateOrderStatusAsync(string userId, int orderId, OrderStatus newStatus);
        Task<ApiResponse<IReadOnlyList<OrderSummaryDto>>> GetMyOrdersAsFarmerAsync(string farmerId);

        //// Admin actions
        Task<ApiResponse<Pagination<OrderSummaryDto>>> GetAllOrdersAsync(OrderSpecParams specs);

        // any one
        Task<ApiResponse<DeliveryMethod>> GetDeliveryMethod(int deliveryMethodId);
    }
}
