using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities
{
    public class ApplicationUser : IdentityUser 
    {
        public string Name { get; set; } = string.Empty;  
        public string? ProfileImageUrl { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        // Navigation
        public virtual FarmerProfile? FarmerProfile { get; set; }
        public virtual TraderProfile? TraderProfile { get; set; }
        public virtual ICollection<Address> Addresses { get; set; } = new HashSet<Address>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
    }
}
