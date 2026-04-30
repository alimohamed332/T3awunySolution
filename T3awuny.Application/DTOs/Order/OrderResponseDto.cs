using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.OrderAggregate;

namespace T3awuny.Application.DTOs.Order
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string BuyerName { get; set; } = string.Empty;//
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public OrderAddress DeliveryAddress { get; set; } = default!;
        public List<OrderItemResponseDto> Items { get; set; } = new List<OrderItemResponseDto>();//
        //public PaymentResponseDto? Payment { get; set; }
        //public LogisticsResponseDto? Logistics { get; set; }
    }

    public class OrderItemResponseDto
    {
        public int ProductId { get; set; }//
        public string ProductName { get; set; } = string.Empty;//
        public string? MainImageUrl { get; set; }//
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;//
        public decimal UnitPriceAtOrder { get; set; }
        public decimal Subtotal { get; set; }
    }
}
