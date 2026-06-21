using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Application.DTOs.AI_Dtos
{
    public class AIPaymentDto
    {
        public int Id { get; set; }
        public string Method { get; set; } = string.Empty;///
        public string Status { get; set; } = string.Empty;///
        public decimal Amount { get; set; }
        public DateTime? PaidAt { get; set; }

    }
}
