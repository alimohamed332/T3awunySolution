using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities.OrderAggregate
{
    public class ProductItemOrdered
    {
        public ProductItemOrdered() { }
        public ProductItemOrdered(int productId, string productName, string pictureUrl, string unit)
        {
            ProductId = productId;
            ProductName = productName;
            PictureUrl = pictureUrl;
            Unit = unit;
        }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string PictureUrl { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
    }
}
