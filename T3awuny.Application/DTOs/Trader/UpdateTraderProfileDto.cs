using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Application.DTOs.Trader
{
    public class UpdateTraderProfileDto
    {
        public string? UserId { get; set; }
        public string? BusinessName { get; set; }
        public BusinessType BusinessType { get; set; } = BusinessType.Wholesaler;
        public string? Description { get; set; }
        public string? Name { get; set; } 
    }
}
