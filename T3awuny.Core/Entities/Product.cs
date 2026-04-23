using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string FarmerId { get; set; } = string.Empty;         // FK → ApplicationUser
        public virtual ApplicationUser Farmer { get; set; } = null!;
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;      // kg, ton, box, piece
        public decimal UnitPrice { get; set; }
        public DateTime? HarvestDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public ProductStatus Status { get; set; }    // enum
        public virtual DateTime CreatedAt { get; set; }
        public virtual ICollection<ProductImage> Images { get; set; } = new HashSet<ProductImage>();
    }
}
