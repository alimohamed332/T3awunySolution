using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Application.DTOs.AI_Dtos
{
    public class AIReviewDto
    {
        public int Id { get; set; }
        public string ReviewerId { get; set; } = string.Empty;
        public string ReviewerName { get; set; } = string.Empty;//
        public string TargetFarmerId { get; set; } = string.Empty;//
        public string TargetFarmerName { get; set; } = string.Empty;//
        public int? OrderId { get; set; }
        public int Rating { get; set; }           // 1 to 5
        public string? Comment { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
