using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.ProductModule;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Application.DTOs.AI_Dtos
{
    public class AIProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;      // kg, ton, box, piece
        public decimal UnitPrice { get; set; }
        public string Status { get; set; } = string.Empty;//   
        public string Governorate { get; set; } = string.Empty;     // 
        public int CategoryId { get; set; }
        public string FarmerId { get; set; } = string.Empty;      
        public DateTime? HarvestDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        

    }
}

