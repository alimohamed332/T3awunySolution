using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Application.DTOs.AI_Dtos
{
    public class AITraderProfileDto
    {
        public string BusinessName { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty;//
        public string TaxNumber { get; set; } = string.Empty;
    }
}
