using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Application.DTOs.AI_Dtos
{
    public class AIOrderDto
    {
        public int Id { get; set; }
        public string? BuyerId { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public string? FarmerId { get; set; }
        public decimal TotalAmount { get; set; }//
        public string PaymentStatus { get; set; } = string.Empty;//
        public string Status { get; set; } = string.Empty;//
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public AIOrderAddress DeliveryAddress { get; set; } = new AIOrderAddress();//

        public List<AIItemDto> Items { get; set; } = new List<AIItemDto>();
        public AIPaymentDto Payment { get; set; } = new AIPaymentDto();
        public AILogisticsDto Logistics { get; set; } = new AILogisticsDto();

    }
}