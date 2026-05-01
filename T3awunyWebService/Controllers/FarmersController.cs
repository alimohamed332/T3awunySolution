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
    public class FarmersController : ControllerBase
    {
        private readonly IFarmerService _farmerService;

        public FarmersController(IFarmerService farmerService)
        {
            _farmerService = farmerService;
        }

        [Authorize]
        [HttpGet("farmer-profiles/{id?}")]
        public async Task<ActionResult<ApiResponse<FarmerProfileDto>>> GetFarmerProfile(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                // Get the user ID from the claims if not provided in the route => to get the profile of the currently authenticated farmer
                id = GetUserIdFromClaims();
                if (string.IsNullOrEmpty(id))
                    return BadRequest(ApiResponse<FarmerProfileDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            }
            var farmerProfile = await _farmerService.GetProfileAsync(id);
            if (farmerProfile is null) 
                return NotFound(ApiResponse<FarmerProfileDto>.Fail(" هذا المستخدم لا يملك بروفايل مزارع"));

            return Ok(ApiResponse<FarmerProfileDto>.Ok(farmerProfile, "تم العثور على بروفايل المزارع بنجاح"));
        }

        [Authorize("FarmerOrAdmin")]
        [HttpPost("farmer-profile")]
        public async Task<ActionResult<ApiResponse<FarmerProfileDto>>> CreateFarmerProfile([FromBody]CreateFarmerProfileDto dto)
        {
            var id = dto.UserId;
            if (string.IsNullOrEmpty(id))
            {
                // Get the user ID from the claims if not provided in the route => to get the profile of the currently authenticated farmer
                id = GetUserIdFromClaims();
                if (string.IsNullOrEmpty(id))
                    return BadRequest(ApiResponse<TraderProfileDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            }
            var farmerProfile = await _farmerService.CreateProfileAsync(id, dto);
            if (farmerProfile.Messsage is not null) 
                return BadRequest(ApiResponse<FarmerProfileDto>.Fail(farmerProfile.Messsage));

            return Ok(ApiResponse<FarmerProfileDto>.Ok(farmerProfile, "تم إنشاء البروفايل بنجاح"));
        }

        [Authorize("FarmerOrAdmin")]
        [HttpPut("farmer-profile")]
        public async Task<ActionResult<ApiResponse<FarmerProfileDto>>> UpdateFarmerProfile([FromBody]UpdateFarmerProfileDto dto)
        {
            var id = dto.UserId;
            if (string.IsNullOrEmpty(id))
            {
                // Get the user ID from the claims if not provided in the route => to get the profile of the currently authenticated farmer
                id = GetUserIdFromClaims();
                if (string.IsNullOrEmpty(id))
                    return BadRequest(ApiResponse<TraderProfileDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            }
            var farmerProfile = await _farmerService.UpdateProfileAsync(id, dto);
            if (farmerProfile.Messsage is not null) 
                return BadRequest(ApiResponse<FarmerProfileDto>.Fail(farmerProfile.Messsage));

            return Ok(ApiResponse<FarmerProfileDto>.Ok(farmerProfile, "تم تحديث البروفايل بنجاح"));
        }
   

        private string GetUserIdFromClaims()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? string.Empty;
        }
    }
}