using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Admin;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.DTOs.Trader;
using T3awuny.Application.DTOs.User;
using T3awuny.Application.Helpers;
using T3awuny.Application.Services;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.UserModule;
using T3awuny.Core.Specifications.UserSpecs;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("AdminOnly")]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;
        //private readonly IFarmerService _farmerService;
        //private readonly ITraderService _traderService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IMemoryCache _cache;

        public AdminsController(IAdminService adminService, /*IFarmerService farmerService, ITraderService traderService,*/ IUserService userService, IProductService productService, IMemoryCache cache)
        {
            _adminService = adminService;
            //_farmerService = farmerService;
            //_traderService = traderService;
            _userService = userService;
            _productService = productService;
            _cache = cache;
        }


        [HttpGet("dashboard-report")]
        public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetAdminDashboardStats()
        {
            var result = await _adminService.GetDashboardStatsAsync();
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Admin only endpoint to retrieve a paginated list of all users on the platform, with optional filtering and sorting.
        /// IsActive is the user banned or not
        /// IsVerified is the user profile verified by admin or still pending
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<Pagination<AdminUserDto>>>> GetAllUsers([FromQuery] AdminUserFilterDto filter)
        {
            var result = await _adminService.GetAllUsersAsync(filter);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

       // [Authorize("AdminOnly")]
        [HttpGet("admin/{id}")]
        public async Task<ActionResult<ApiResponse<AdminUserDto>>> GetAdminById(string id)
        {
            var result = await _adminService.GetAdminByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        //[Authorize("AdminOnly")]
        [HttpGet("my-profile")]
        public async Task<ActionResult<ApiResponse<AdminProfileDto>>> GetMyProfile()
        {
            var adminId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(adminId))
                return BadRequest(ApiResponse<AdminProfileDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            var result = await _adminService.GetMyProfileAsAdmin(adminId);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        //[Authorize("AdminOnly")]
        [HttpPut]
        public async Task<ActionResult<ApiResponse<AdminProfileDto>>> UpdateProfile(string? adminId, UpdateAdminProfileDto dto)
        {
            if (string.IsNullOrEmpty(adminId))
            {
                adminId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(adminId))
                    return BadRequest(ApiResponse<AdminProfileDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            }
            var result = await _adminService.UpdateMyProfileAsAdmin(adminId, dto);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        //[Authorize("AdminOnly")]
        [HttpGet("users/{id}")]
        public async Task<ActionResult<ApiResponse<AdminUserDto>>> GetUserById(string id)
        {
            var result = await _adminService.GetUserByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Admin only endpoint to verify a farmer's account. This action will change the farmer's status to "Verified" and allow them to access all platform features.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize("AdminOnly")]
        [HttpPatch("verify-farmer/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> VerifyFarmer(string id)
        {
            var result = await _adminService.VerifyFarmerAsync(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Admin only endpoint to verify a trader's account. This action will change the trader's status to "Verified" and allow them to access all platform features.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize("AdminOnly")]
        [HttpPatch("verify-trader/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> VerifyTrader(string id)
        {
            var result = await _adminService.VerifyTraderAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        /// <summary>
        /// Admin only endpoint to toggle a user's active status. This can be used to ban or unban a user from the platform. If the user is currently active, they will be banned and unable to log in. If the user is currently banned, they will be reactivated and able to log in again.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize("AdminOnly")]
        [HttpPatch("toggle-user-status/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> ToggleUserStatus( string id)
        {
            var result = await _adminService.ToggleUserStatusAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            if (result.Data == "تم حظر المستخدم بنجاح")
            {
                _cache.Set("BannedUserId", id, TimeSpan.FromHours(1));
                Response.Cookies.Delete("refreshToken");
            }
                
            return Ok(result);
        }

        //[Authorize("AdminOnly")]
        [HttpGet("pending-farmers")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<FarmerProfileDto>>>> GetPendingFarmers()
        {
            var result = await _adminService.GetPendingFarmersAsync();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //[Authorize("AdminOnly")]
        [HttpGet("pending-traders")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<TraderProfileDto>>>> GetPendingTraders()
        {
            var result = await _adminService.GetPendingTradersAsync();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //[Authorize("AdminOnly")]
        [HttpGet("banned-users")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<FarmerProfileDto>>>> GetBannedUsers()
        {
            var result = await _adminService.GetBannedUsersAsync();
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        #region Farmer & Trader controllers
        //[Authorize("AdminOnly")]
        //[HttpGet("verified-farmers")]
        //public async Task<ActionResult<ApiResponse<IReadOnlyList<FarmerProfileDto>>>> GetVerifiedFarmers()
        //{
        //    var verifiedFarmers = await _farmerService.GetAllVerifiedAsync();
        //    if (!verifiedFarmers.IsSuccess)
        //        return NotFound(verifiedFarmers);

        //    return Ok(verifiedFarmers);
        //}

        //[Authorize]
        //[HttpGet("verified-traders")]
        //public async Task<ActionResult<ApiResponse<IReadOnlyList<TraderProfileDto>>>> GetVerifiedTraders()
        //{
        //    var verifiedTraders = await _traderService.GetAllVerifiedAsync();
        //    if (!verifiedTraders.IsSuccess)
        //        return NotFound(verifiedTraders);

        //    return Ok(verifiedTraders);
        //}
        #endregion

        //[Authorize("AdminOnly")]
        [HttpGet("verified-users")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<UserDetailsDto>>>> GetAllVerifiedUsersAsync()
        {
            var result = await _userService.GetAllVerifiedUsersAsync();
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        //[Authorize("AdminOnly")]
        [HttpGet("non-verified-users")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<UserDetailsDto>>>> GetAllNonVerifiedUsersAsync()
        {
            var result = await _userService.GetAllNonVerifiedUsersAsync();
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

       

        //[Authorize("AdminOnly")]
        [HttpDelete("users/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(string id)
        {
            var result = await _adminService.DeleteUserAsync(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Admin only endpoint to flag a product for review. This action will change the product's status to "UnderReview" or return it to active status if it is already under review. This allows admins to manage product listings and ensure they meet platform standards.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        //[Authorize("AdminOnly")]
        [HttpPut("products/{productId}/review")]
        public async Task<ActionResult<ApiResponse<string>>> FlagProduct(int productId)
        {
            var adminId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(adminId))
                return BadRequest(ApiResponse<string>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            var result = await _productService.ChangeStatusAsync(adminId, productId, ProductStatus.UnderReview);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        
    }
}
