using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;

namespace T3awuny.Application.DTOs.Address
{
    public class CreateAddressDto
    {
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        public bool IsDefault { get; set; }
        public AddressLabel Label { get; set; } = AddressLabel.Farm;
    }
}
