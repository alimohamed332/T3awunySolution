using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Application.DTOs.Product
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public DateTime HarvestDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public ProductStatus Status { get; set; } 
        public DateTime CreatedAt { get; set; }

        // Related data
        public string CategoryName { get; set; } = string.Empty;
        public string FarmerName { get; set; } = string.Empty;
        public string FarmName { get; set; } = string.Empty;
        public string FarmerGovernorate { get; set; } = string.Empty;
        public List<string>? ImageUrls { get; set; } 
        public string? MainImageUrl { get; set; }
    }
}
