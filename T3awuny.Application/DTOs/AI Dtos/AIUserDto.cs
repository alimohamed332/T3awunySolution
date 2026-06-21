
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Application.DTOs.AI_Dtos
{
    public class AIUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;//
        public string Role { get; set; } = string.Empty;//
        public string? ProfileImageUrl { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
        //public bool IsVerified { get; set; }
        public  AIFarmerProfileDto? FarmerProfile { get; set; } = default!;
        public  AITraderProfileDto? TraderProfile { get; set; } = default!;
    }
}
