using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities.OrderAggregate
{
    public class OrderItem : BaseEntity
    {
        public OrderItem() {}
        public OrderItem(ProductItemOrdered itemOrdered, decimal quantity, decimal unitPriceAtOrder, decimal subtotal)
        {
            ItemOrdered = itemOrdered;
            Quantity = quantity;
            UnitPriceAtOrder = unitPriceAtOrder;
            Subtotal = subtotal;
        }

        public int Id { get; set; }
        //public int OrderId { get; set; }
        //public Order Order { get; set; }
        public ProductItemOrdered ItemOrdered { get; set; } = default!;
        public decimal Quantity { get; set; }
        public decimal UnitPriceAtOrder { get; set; }
        public decimal Subtotal { get; set; }           // Quantity * UnitPriceAtOrder
    }
}
