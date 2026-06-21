using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Application.DTOs.AI_Dtos
{
    public class AILogisticsDto
    {
        public int Id { get; set; }
        public string? DriverName { get; set; }
        public string? DriverPhone { get; set; }
        public LogisticsStatus Status { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime? ActualDelivery { get; set; } 
       
    }
}
