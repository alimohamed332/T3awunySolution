using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.DTOs.Auction;
using T3awuny.Application.DTOs.Category;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.DTOs.Order;
using T3awuny.Application.DTOs.Product;
using T3awuny.Application.DTOs.Trader;
using T3awuny.Application.DTOs.User;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Application.Contracts
{
    public interface IAdminService
    {
        Task<ApiResponse<bool>> VerifyFarmerAsync(string farmerId);
        Task<ApiResponse<bool>> VerifyTraderAsync(string traderId);
        Task<ApiResponse<string>> ToggleUserStatusAsync(string userId);
        //Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<ApiResponse<IReadOnlyList<FarmerProfileDto>>> GetPendingFarmersAsync();
        Task<ApiResponse<IReadOnlyList<TraderProfileDto>>> GetPendingTradersAsync();
        Task<ApiResponse<IReadOnlyList<BannedUserDto>>> GetBannedUsersAsync();
        Task<ApiResponse<bool>> DeleteUserAsync(string userId);
        Task<ApiResponse<ApplicationUser>> GetAdminByIdAsync(string adminId);
        Task<ApiResponse<ApplicationUser>> GetUserByIdAsync(string userId);
        #region Admin
        //// ── Dashboard ────────────────────────────────────────
        //Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync();

        //// ── User Management ──────────────────────────────────
        //Task<ApiResponse<PagedResult<AdminUserDto>>> GetAllUsersAsync(
        //    AdminUserFilterDto filter);
        //Task<ApiResponse<AdminUserDto>> GetUserDetailsAsync(string userId);
        //Task<ApiResponse<string>> ToggleUserStatusAsync(string userId);
        //Task<ApiResponse<string>> VerifyFarmerAsync(string userId);
        //Task<ApiResponse<string>> VerifyTraderAsync(string userId);
        //Task<ApiResponse<IEnumerable<AdminUserDto>>> GetPendingVerificationsAsync();
        //Task<ApiResponse<string>> DeleteUserAsync(string userId);

        //// ── Product Management ───────────────────────────────
        //Task<ApiResponse<PagedResult<ProductResponseDto>>> GetAllProductsAsync(
        //    AdminProductFilterDto filter);
        //Task<ApiResponse<string>> ChangeProductStatusAsync(
        //    int productId, ProductStatus status);
        //Task<ApiResponse<string>> DeleteProductAsync(int productId);

        //// ── Order Management ─────────────────────────────────
        //Task<ApiResponse<PagedResult<OrderSummaryDto>>> GetAllOrdersAsync(
        //    AdminOrderFilterDto filter);
        //Task<ApiResponse<string>> UpdateOrderStatusAsync(
        //    int orderId, OrderStatus status);

        //// ── Auction Management ───────────────────────────────
        //Task<ApiResponse<PagedResult<AuctionSummaryDto>>> GetAllAuctionsAsync(
        //    AdminAuctionFilterDto filter);
        //Task<ApiResponse<string>> CancelAuctionAsync(int auctionId, string reason);

        //// ── Community Moderation ─────────────────────────────
        //Task<ApiResponse<PagedResult<PostSummaryDto>>> GetAllPostsAsync(
        //    int page, int pageSize, bool deletedOnly = false);
        //Task<ApiResponse<string>> DeletePostAsync(int postId);
        //Task<ApiResponse<string>> DeleteCommentAsync(int commentId);

        //// ── Category Management ──────────────────────────────
        //Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllCategoriesAsync();
        //Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto);
        //Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(
        //    int id, CreateCategoryDto dto);
        //Task<ApiResponse<string>> DeleteCategoryAsync(int id);

        //// ── Delivery Methods ─────────────────────────────────
        //Task<ApiResponse<IEnumerable<DeliveryMethodDto>>> GetDeliveryMethodsAsync();
        //Task<ApiResponse<DeliveryMethodDto>> CreateDeliveryMethodAsync(
        //    CreateDeliveryMethodDto dto);
        //Task<ApiResponse<string>> DeleteDeliveryMethodAsync(int id);
        #endregion
    }
}
