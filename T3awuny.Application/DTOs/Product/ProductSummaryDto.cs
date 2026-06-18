using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Application.DTOs.Product
{
    public class ProductSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; } = string.Empty;
        public ProductStatus Status { get; set; }
        public string FarmerId { get; set; } = string.Empty;
        public string? FarmerName { get; set; } ///////
        public string FarmerGovernorate { get; set; } = string.Empty; ///////
        public string FarmerCity { get; set; } = string.Empty; /////
        public string? MainImageUrl { get; set; }
        public bool HasActiveAcution { get; set; }
    }
}
