using T3awuny.Application.Common;
using T3awuny.Application.DTOs.Admin;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.DTOs.Product;
using T3awuny.Application.DTOs.Trader;
using T3awuny.Application.DTOs.User;
using T3awuny.Application.Helpers;
using T3awuny.Core.Entities.UserModule;
using T3awuny.Core.Specifications.UserSpecs;

namespace T3awuny.Application.Contracts
{
    public interface IAdminService
    {      
        #region Admin
        //// ── Dashboard ────────────────────────────────────────
        Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync();

        //// ── User Management ──────────────────────────────────
        Task<ApiResponse<Pagination<AdminUserDto>>> GetAllUsersAsync(AdminUserFilterDto filter);
        Task<ApiResponse<AdminUserDto>> GetUserByIdAsync(string userId);
        Task<ApiResponse<AdminUserDto>> GetAdminByIdAsync(string adminId);
        Task<ApiResponse<string>> ToggleUserStatusAsync(string userId);
        Task<ApiResponse<bool>> VerifyFarmerAsync(string farmerId);
        Task<ApiResponse<bool>> VerifyTraderAsync(string traderId);
        //Task<ApiResponse<IEnumerable<AdminUserDto>>> GetPendingVerificationsAsync();
        Task<ApiResponse<IReadOnlyList<FarmerProfileDto>>> GetPendingFarmersAsync();////////
        Task<ApiResponse<IReadOnlyList<TraderProfileDto>>> GetPendingTradersAsync();/////////
        Task<ApiResponse<IReadOnlyList<BannedUserDto>>> GetBannedUsersAsync();//////////
        Task<ApiResponse<bool>> DeleteUserAsync(string userId);////////

        //// ── Product Management ───────────────────────────────
        //getall , getbyid delete , change status

        //// ── Order Management ─────────────────────────────────
        // get all orders with filter, get by id, change status

        //// ── Auction Management ───────────────────────────────
        // get all auctions with filter, get by id, cancel auction 

        //// ── Community Moderation ─────────────────────────────
        //Task<ApiResponse<PagedResult<PostSummaryDto>>> GetAllPostsAsync(int page, int pageSize, bool deletedOnly = false);
        //Task<ApiResponse<string>> DeletePostAsync(int postId);
        //Task<ApiResponse<string>> DeleteCommentAsync(int commentId);

        //// ── Category Management ──────────────────────────────
        // getall , getbyid, delete , create , update

        //// ── Delivery Methods ─────────────────────────────────
        //Task<ApiResponse<IEnumerable<DeliveryMethodDto>>> GetDeliveryMethodsAsync();
        //Task<ApiResponse<DeliveryMethodDto>> CreateDeliveryMethodAsync(CreateDeliveryMethodDto dto);
        //Task<ApiResponse<string>> DeleteDeliveryMethodAsync(int id);
        #endregion
    }
}
