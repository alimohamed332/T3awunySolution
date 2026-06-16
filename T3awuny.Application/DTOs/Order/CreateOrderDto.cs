using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Application.DTOs.Order
{
    public class CreateOrderDto
    {
        //public int DeliveryAddressId { get; set; }
        public string? Notes { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        //public List<CreateOrderItemDto> Items { get; set; }
        [Required]
        public string BasketId { get; set; } = string.Empty;
    }

    //public class CreateOrderItemDto
    //{
    //    public int ProductId { get; set; }
    //    public decimal Quantity { get; set; }
    //}
}
