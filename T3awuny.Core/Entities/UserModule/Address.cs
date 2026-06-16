using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Core.Entities.UserModule
{
    public class Address : BaseEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty; //(FK → AspNetUsers)
        public AddressLabel Label { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Governorate { get; set; } = string.Empty;
        public int? PostalCode { get; set; }
        public string Country { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsDefault { get; set; } 

    }
}
