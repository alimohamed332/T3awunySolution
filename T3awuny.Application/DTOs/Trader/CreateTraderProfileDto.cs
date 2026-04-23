using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Application.DTOs.Trader
{
    public class CreateTraderProfileDto
    {
        public string? UserId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public BusinessType BusinessType { get; set; } = BusinessType.Wholesaler;  // Restaurant, Wholesaler, Retailer
        public string? Description { get; set; }
        public int? TaxNumber { get; set; }
    }

}
