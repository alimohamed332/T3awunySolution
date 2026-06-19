using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.AI_Dtos
{
    public class AIFarmerProfileDto
    {
        public string FarmName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double FarmSize { get; set; } = 5.0;
        public string Governorate { get; set; } = string.Empty; //
    }
}
