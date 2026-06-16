using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Application.DTOs.Order
{
    public class OrderLogisticsResponseDto
    {
        public int LogisticsId { get; set; }
        public string LogisticsStatus { get; set; } = string.Empty;
        public DateTime? EstimatedDelivery { get; set; }
        public string? Notes { get; set; } 
    }
}
