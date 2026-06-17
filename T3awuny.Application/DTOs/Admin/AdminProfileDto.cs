using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.DTOs.Address;

namespace T3awuny.Application.DTOs.Admin
{
    public class AdminProfileDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public AddressDetailsDto Address { get; set; } = new AddressDetailsDto();
        public string? ProfileImageUrl { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public bool IsVerified { get; set; }
        public string? Messsage { get; set; }
    }
}
