using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities
{
    public class TraderProfile
    {
        public string TraderId { get; set; } = string.Empty; //(PK, FK → AspNetUsers)
        public string? BusinessName { get; set; } = string.Empty;
        public BusinessType? BusinessType { get; set; }
        public int? TaxNumber { get; set; }
        public bool IsVerified { get; set; }
        public DateTime VerifiedAt { get; set; }
    }
}