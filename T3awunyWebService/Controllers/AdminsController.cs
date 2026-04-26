using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.DTOs.Trader;
using T3awuny.Application.DTOs.User;
using T3awuny.Application.Services;
using T3awuny.Core.Entities;
using T3awuny.Core.Entities.Enums;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IFarmerService _farmerService;
        private readonly ITraderService _traderService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public AdminsController(IAdminService adminService, IFarmerService farmerService, ITraderService traderService, IUserService userService, IProductService productService)
        {
            _adminService = adminService;
            _farmerService = farmerService;
            _traderService = traderService;
            _userService = userService;
            _productService = productService;
        }
        [Authorize("AdminOnly")]
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

        [Authorize("AdminOnly")]
        [HttpPatch("verify-trader/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> VerifyTrader(string id)
        {
            var result = await _adminService.VerifyTraderAsync(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize("AdminOnly")]
        [HttpPatch("toggle-user-status/{id}")]
        public async Task<ActionResult<ApiResponse<string>>> ToggleUserStatus( string id)
        {
            var result = await _adminService.ToggleUserStatusAsync(id);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [Authorize("AdminOnly")]
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

        [Authorize("AdminOnly")]
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

        [Authorize("AdminOnly")]
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

        [Authorize("AdminOnly")]
        [HttpGet("verified-farmers")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<FarmerProfileDto>>>> GetVerifiedFarmers()
        {
            var verifiedFarmers = await _farmerService.GetAllVerifiedAsync();
            if (!verifiedFarmers.IsSuccess)
                return NotFound(verifiedFarmers);

            return Ok(verifiedFarmers);
        }

        [Authorize]
        [HttpGet("verified-traders")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<TraderProfileDto>>>> GetVerifiedTraders()
        {
            var verifiedTraders = await _traderService.GetAllVerifiedAsync();
            if (!verifiedTraders.IsSuccess)
                return NotFound(verifiedTraders);

            return Ok(verifiedTraders);
        }

        [Authorize("AdminOnly")]
        [HttpGet("verified-users")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<UserDetailsDto>>>> GetAllVerifiedUsersAsync()
        {
            var result = await _userService.GetAllVerifiedUsersAsync();
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [Authorize("AdminOnly")]
        [HttpGet("non-verified-users")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<UserDetailsDto>>>> GetAllNonVerifiedUsersAsync()
        {
            var result = await _userService.GetAllNonVerifiedUsersAsync();
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        [Authorize("AdminOnly")]
        [HttpGet("admin/{id}")]
        public async Task<ActionResult<ApiResponse<ApplicationUser>>> GetAdminById(string id)
        {
            var result = await _adminService.GetAdminByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [Authorize("AdminOnly")]
        [HttpGet("users/{id}")]
        public async Task<ActionResult<ApiResponse<ApplicationUser>>> GetUserById(string id)
        {
            var result = await _adminService.GetUserByIdAsync(id);
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [Authorize("AdminOnly")]
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

        [Authorize("AdminOnly")]
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
