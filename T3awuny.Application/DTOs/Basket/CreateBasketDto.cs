using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.BasketModule;

namespace T3awuny.Application.DTOs.Basket
{
    public class CreateBasketDto
    {
        public string? Id { get; set; }  // this now the userId to bind each basket to user
        public ICollection<BasketItem> Items { get; set; } = [];
        public int? DeliveryMethodId { get; set; }
    }
}
