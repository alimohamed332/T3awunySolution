using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;

namespace T3awuny.Application.DTOs.Logistics
{
    public class LogisticsResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int PickupAddressId { get; set; }
        public OrderAddress PickupAddress { get; set; } = new OrderAddress();//
        public int DeliveryAddressId { get; set; }
        public OrderAddress DeliveryAddress { get; set; } = new OrderAddress();//
        public string? DriverName { get; set; }
        public string? DriverPhone { get; set; }
        public LogisticsStatus Status { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime? ActualDelivery { get; set; } //
        public string? Notes { get; set; } //
    }
}
