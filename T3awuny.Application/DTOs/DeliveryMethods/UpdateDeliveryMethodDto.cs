using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.DeliveryMethods
{
    public class UpdateDeliveryMethodDto
    {
        public string? ShortName { get; set; } 
        public string? Description { get; set; } 
        public decimal? Cost { get; set; }
        public string? DeliveryTime { get; set; } 
    }
}
