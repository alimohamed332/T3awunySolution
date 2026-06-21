using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.OrderAggregate;

namespace T3awuny.Application.DTOs.AI_Dtos
{
    public class AIItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; } //
        public string ProductName { get; set; } = string.Empty;//
        public decimal Quantity { get; set; }
        public decimal UnitPriceAtOrder { get; set; }
        public decimal Subtotal { get; set; }           // Quantity * UnitPriceAtOrder
    }
}
