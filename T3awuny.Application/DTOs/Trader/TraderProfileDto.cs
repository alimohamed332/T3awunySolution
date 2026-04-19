using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.DTOs.Address;

namespace T3awuny.Application.DTOs.Trader
{
    public class TraderProfileDto
    {
        public string TraderId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? BusinessName { get; set; } = string.Empty;
        public string? BusinessType { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<AddressDetailsDto> Addresses { get; set; } = new List<AddressDetailsDto>();
        public string? ProfileImageUrl { get; set; } = string.Empty;
        public int? TaxNumber { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsVerified { get; set; }
        public string? Messsage { get; set; }
    }
}
