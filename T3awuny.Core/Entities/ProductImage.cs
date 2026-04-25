using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities
{
    public class ProductImage : BaseEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }    
        public int? DisplayOrder { get; set; }
    }
}
