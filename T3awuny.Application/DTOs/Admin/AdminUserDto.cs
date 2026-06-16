using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Admin
{
    public class AdminUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;//
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        public DateTime JoinDate { get; set; }
        public string? ProfileImageUrl { get; set; }//

        // Role-specific
        //public string? FarmName { get; set; }          // Farmer only
        //public string? BusinessName { get; set; }       // Trader only

        // Activity stats
        public int TotalOrders { get; set; }//
        public int TotalProducts { get; set; }//         // Farmer only
        //public decimal TotalSpent { get; set; }        // Trader
    }
}
