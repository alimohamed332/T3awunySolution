using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Farmer
{
    public class CreateFarmerProfileDto
    {
        public string FarmName { get; set; } = string.Empty;
        public string? Description { get; set; }
        //public double? Latitude { get; set; }   // reverse geocode for farm address
        //public double? Longitude { get; set; }
        //public decimal? FarmSize { get; set; }
    }
}
