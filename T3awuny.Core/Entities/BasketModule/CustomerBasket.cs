using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities.BasketModule
{
    public class CustomerBasket
    {
        public string Id { get; set; } = default!; //GUID  Generated in client side or server side still not decided
        public ICollection<BasketItem> Items { get; set; } = [];
        public int? DeliveryMethodId {  get; set; } 
        public decimal ShippingPrice { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }
    }
}
