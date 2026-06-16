using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Application.DTOs.Logistics
{
    public class UpdateLogisticsDto
    {
        public int? PickupAddressId { get; set; }
        public int? DeliveryAddressId { get; set; }
        public string? DriverName { get; set; }
        public string? DriverPhone { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime? ActualDelivery { get; set; } 
        public string? Notes { get; set; } 
    }
}
