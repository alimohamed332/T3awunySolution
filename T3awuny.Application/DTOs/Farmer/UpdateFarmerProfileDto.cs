using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Farmer
{
    public class UpdateFarmerProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string? FarmName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
    }
}
