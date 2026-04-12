using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;

namespace T3awuny.Core.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty; //(FK → AspNetUsers)
        public AddressLabel Label { get; set; }
        public string? Street { get; set; } 
        public string? City { get; set; } 
        public string? Governorate { get; set; } 
        public int? PostalCode { get; set; }
        public string? Country { get; set; } = "Egypt";
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool IsDefault { get; set; } 

    }
}
