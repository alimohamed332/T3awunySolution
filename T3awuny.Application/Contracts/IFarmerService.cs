using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.DTOs.Farmer;

namespace T3awuny.Application.Contracts
{
    public interface IFarmerService
    {
        Task<FarmerProfileDto> CreateProfileAsync(string userId, CreateFarmerProfileDto dto);
        Task<FarmerProfileDto> UpdateProfileAsync(string userId, UpdateFarmerProfileDto dto);
        Task<FarmerProfileDto?> GetProfileAsync(string userId);
        Task<ApiResponse<IReadOnlyList<FarmerProfileDto>>> GetAllVerifiedAsync();
    }
}
