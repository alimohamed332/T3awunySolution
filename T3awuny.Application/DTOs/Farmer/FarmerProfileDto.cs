using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Address;

namespace T3awuny.Application.DTOs.Farmer
{
    public class FarmerProfileDto 
    {
        public string FarmerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? FarmName { get; set; } = string.Empty; 
        public string? Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<AddressDetailsDto> Addresses { get; set; } = new List<AddressDetailsDto>();
        public string? ProfileImageUrl { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public bool IsVerified { get; set; }
        public string? Messsage { get; set; }
    }
}
