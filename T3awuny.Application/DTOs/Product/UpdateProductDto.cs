using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Product
{
    public class UpdateProductDto
    {
        [Required]
        public int Id { get; set; }
        public string? Name { get; set; } 
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public decimal? Quantity { get; set; }
        public string? Unit {  get; set; }
        public decimal? UnitPrice{ get; set; }
        public DateTime? HarvestDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

    }
}
