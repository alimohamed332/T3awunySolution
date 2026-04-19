using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.DTOs.Trader;
using T3awuny.Application.DTOs.User;

namespace T3awuny.Application.Contracts
{
    public interface IAdminService
    {
        Task<ApiResponse<bool>> VerifyFarmerAsync(string farmerId);
        Task<ApiResponse<bool>> VerifyTraderAsync(string traderId);
        Task<ApiResponse<string>> ToggleUserStatusAsync(string userId);
        //Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<ApiResponse<IEnumerable<FarmerProfileDto>>> GetPendingFarmersAsync();
        Task<ApiResponse<IEnumerable<TraderProfileDto>>> GetPendingTradersAsync();
        Task<ApiResponse<IEnumerable<BannedUserDto>>> GetBannedUsersAsync();

    }
}
