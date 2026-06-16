using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Product
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        public DateTime? HarvestDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
        public bool PublishImmediately { get; set; } = true;
        [Required]
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}




