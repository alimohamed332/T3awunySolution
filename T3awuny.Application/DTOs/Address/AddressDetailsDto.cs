using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Address
{
    public class AddressDetailsDto
    {
        public int? Id { get; set; }
        public string? UserId { get; set; } 
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Governorate { get; set; } =  string.Empty;
        public int? PostalCode { get; set; } 
        public string Country { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
