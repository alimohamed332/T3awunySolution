using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.DTOs.Trader;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradersController : ControllerBase
    {
        private readonly ITraderService _traderService;

        public TradersController(ITraderService traderService)
        {
            _traderService = traderService;
        }

        [Authorize]
        [HttpGet("trader-profile/{id?}")]
        public async Task<ActionResult<ApiResponse<TraderProfileDto>>> GetTraderProfile(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                // Get the user ID from the claims if not provided in the route => to get the profile of the currently authenticated farmer
                id = GetUserIdFromClaims();
                if (string.IsNullOrEmpty(id))
                    return BadRequest(ApiResponse<TraderProfileDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            }
            var traderProfile = await _traderService.GetProfileAsync(id);
            if (traderProfile is null)
                return NotFound(ApiResponse<TraderProfileDto>.Fail("هذا المستخدم لا يملك بروفايل تاجر"));
            return Ok(ApiResponse<TraderProfileDto>.Ok(traderProfile, "تم العثور على بروفايل التاجر بنجاح"));
        }

        [Authorize("TraderOrAdmin")]
        [HttpPost("create-profile")]
        public async Task<ActionResult<ApiResponse<TraderProfileDto>>> CreateTraderProfile([FromBody] CreateTraderProfileDto dto)
        {
            var id = dto.UserId;
            if (string.IsNullOrEmpty(id))
            {
                // Get the user ID from the claims if not provided in the route => to get the profile of the currently authenticated farmer
                id = GetUserIdFromClaims();
                if (string.IsNullOrEmpty(id))
                    return BadRequest(ApiResponse<TraderProfileDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            }
            var traderProfile = await _traderService.CreateProfileAsync(id, dto);
            if (traderProfile.Messsage is not null)
                return BadRequest(ApiResponse<TraderProfileDto>.Fail(traderProfile.Messsage));

            return Ok(ApiResponse<TraderProfileDto>.Ok(traderProfile, "تم إنشاء البروفايل بنجاح"));
        }

        [Authorize("TraderOrAdmin")]
        [HttpPut("update-profile")]
        public async Task<ActionResult<ApiResponse<TraderProfileDto>>> UpdateTraderProfile([FromBody] UpdateTraderProfileDto dto)
        {
            var id = dto.UserId;
            if (string.IsNullOrEmpty(id))
            {
                // Get the user ID from the claims if not provided in the route => to get the profile of the currently authenticated farmer
                id = GetUserIdFromClaims();
                if (string.IsNullOrEmpty(id))
                    return BadRequest(ApiResponse<TraderProfileDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            }
            var traderProfile = await _traderService.UpdateProfileAsync(id, dto);
            if (traderProfile.Messsage is not null)
                return BadRequest(ApiResponse<TraderProfileDto>.Fail(traderProfile.Messsage));

            return Ok(ApiResponse<TraderProfileDto>.Ok(traderProfile, "تم تحديث البروفايل بنجاح"));
        }

        private string GetUserIdFromClaims()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? string.Empty;
        }
    }
}
