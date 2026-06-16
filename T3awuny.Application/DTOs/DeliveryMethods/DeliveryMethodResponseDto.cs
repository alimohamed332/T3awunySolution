using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.DeliveryMethods
{
    public class DeliveryMethodResponseDto
    {
        public int Id { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public string DeliveryTime { get; set; } = string.Empty;
    }
}
