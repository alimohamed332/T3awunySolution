using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Core.Entities.UserModule
{
    public class FarmerProfile : BaseEntity
    {
        public string FarmerId { get; set; } = string.Empty; //(PK, FK → AspNetUsers)
        public virtual ApplicationUser? User { get; set; } = null;
        // some info about the farm of the farmer
        public string? FarmName { get; set; }
        //public int FarmSize { get; set; } ali nasr need to remove 
        //public int? FarmAddressId { get; set; }  // (FK → Address)
        //public Address? Address { get; set; }
        public string? Description { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedAt { get; set; }
    }
}