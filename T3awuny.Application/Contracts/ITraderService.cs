using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.DTOs.Trader;

namespace T3awuny.Application.Contracts
{
    public interface ITraderService
    {
        Task<TraderProfileDto> CreateProfileAsync(string userId, CreateTraderProfileDto dto);
        Task<TraderProfileDto> UpdateProfileAsync(string userId, UpdateTraderProfileDto dto);
        Task<TraderProfileDto?> GetProfileAsync(string userId);
        Task<ApiResponse<IEnumerable<TraderProfileDto>>> GetAllVerifiedAsync();
    }
}
