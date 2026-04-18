using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Farmer;

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
        [HttpGet("farmer-profile/{id?}")]
        public async Task<ActionResult<ApiResponse<FarmerProfileDto>>> GetFarmerProfile(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                // Get the user ID from the claims if not provided in the route => to get the profile of the currently authenticated farmer
                id = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
                if (string.IsNullOrEmpty(id))
                    return BadRequest(ApiResponse<FarmerProfileDto>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            }
            var farmerProfile = await _farmerService.GetProfileAsync(id);
            if (farmerProfile is null) 
                return NotFound(ApiResponse<FarmerProfileDto>.Fail("هذا المزارع لا يملك بروفايل"));

            return Ok(ApiResponse<FarmerProfileDto>.Ok(farmerProfile, "تم العثور على بروفايل المزارع بنجاح"));
        }

        [Authorize]
        [HttpPost("create-profile")]
        public async Task<ActionResult<ApiResponse<FarmerProfileDto>>> CreateFarmerProfile(string id, [FromBody]CreateFarmerProfileDto dto)
        {
            var farmerProfile = await _farmerService.CreateProfileAsync(id, dto);
            if (farmerProfile.Messsage is not null) 
                return BadRequest(ApiResponse<FarmerProfileDto>.Fail(farmerProfile.Messsage));

            return Ok(ApiResponse<FarmerProfileDto>.Ok(farmerProfile, "تم إنشاء البروفايل بنجاح"));
        }

        [Authorize]
        [HttpPut("update-profile")]
        public async Task<ActionResult<ApiResponse<FarmerProfileDto>>> UpdateFarmerProfile(string id, [FromBody]UpdateFarmerProfileDto dto)
        {
            var farmerProfile = await _farmerService.UpdateProfileAsync(id, dto);
            if (farmerProfile.Messsage is not null) 
                return BadRequest(ApiResponse<FarmerProfileDto>.Fail(farmerProfile.Messsage));

            return Ok(ApiResponse<FarmerProfileDto>.Ok(farmerProfile, "تم تحديث البروفايل بنجاح"));
        }

        [Authorize]
        [HttpGet("verified-farmers")]
        public async Task<ActionResult<ApiResponse<IEnumerable<FarmerProfileDto>>>> GetVerifiedFarmers()
        {
            var verifiedFarmers = await _farmerService.GetAllVerifiedAsync();
            if (!verifiedFarmers.Any())
                return NotFound(ApiResponse<IEnumerable<FarmerProfileDto>>.Fail("لا يوجد مزارعين موثقين"));

            return Ok(ApiResponse<IEnumerable<FarmerProfileDto>>.Ok(verifiedFarmers, "تم العثور على المزارعين الموثقين بنجاح"));
        }

    }
}