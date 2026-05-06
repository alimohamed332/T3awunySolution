using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Core.Entities.OrderAggregate
{
    public class Order : BaseEntity
    {
        public Order(){}
        public Order(string buyerEmail, string buyerId, decimal subTotal, string? notes, OrderAddress deliveryAddress, ICollection<OrderItem> items, DeliveryMethod? deliveryMethod, string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            BuyerId = buyerId;
            SubTotal = subTotal;
            Notes = notes;
            DliveryAddress = deliveryAddress;
            Items = items;
            DeliveryMethod = deliveryMethod;
            PaymentIntentId = paymentIntentId;
        }

        public int Id { get; set; }
        public string BuyerEmail { get; set; } = string.Empty;
        public string? BuyerId { get; set; } 
        //public virtual ApplicationUser Buyer { get; set; } = default!;
        //public decimal TotalAmount { get; set; }
        public decimal SubTotal { get; set; }
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
        public OrderAddress DliveryAddress { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? FarmerId { get; set; }
        public virtual ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();
        //public int? PaymentId { get; set; }
        public virtual Payment? Payment { get; set; }
        public string PaymentIntentId { get; set; }
        //public int? LogisticsId { get; set; }
        public virtual Logistics? Logistics { get; set; }
        public virtual DeliveryMethod? DeliveryMethod { get; set; }
        public decimal GetTotal()
            => SubTotal + DeliveryMethod?.Cost??0; // calculated property
    }
}
