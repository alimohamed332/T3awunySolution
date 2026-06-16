using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs;
using T3awuny.Application.DTOs.Address;
using T3awuny.Application.DTOs.Trader;
using T3awuny.Application.DTOs.User;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;

        public UsersController(IUserService userService, IAddressService addressService)
        {
            _userService = userService;
            _addressService = addressService;
        }
        [Authorize]
        [HttpPut("profiles/images")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateProfileImageAsync(string? userId,[FromForm] UpdateProfileImageDto dto)
        {
            if (string.IsNullOrEmpty(userId))
            {
                // Get the user ID from the claims if not provided in the route => to get the profile of the currently authenticated farmer
                userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? string.Empty;
                if (string.IsNullOrEmpty(userId))
                    return BadRequest(ApiResponse<string>.Fail("معرف المستخدم غير موجود في الرمز المميز"));
            }
            if (dto.Image is null || dto.Image.Length == 0)
                return BadRequest(ApiResponse<string>.Fail("يرجى تحميل صورة صحيحة"));

            var result = await _userService.UpdateProfileImageAsync(userId, dto.Image);
            if (!result)
                return BadRequest(ApiResponse<string>.Fail("فشل تحديث صورة الملف الشخصي"));

            return Ok(ApiResponse<string>.Ok("", "تم تحديث صورة الملف الشخصي بنجاح"));
        }

        [HttpGet("{userId}/addresses")]
        public async Task<ActionResult<ApiResponse<AddressDetailsDto>>> GetUserAddress(string userId)
        {
            var result = await _addressService.GetAddressByUserIdAsync(userId);
            if (result is null)
                return NotFound(ApiResponse<AddressDetailsDto>.Fail("لم نحصل علي عنوان رئيسي لهذا المستخدم"));
            return Ok(ApiResponse<AddressDetailsDto>.Ok(result,"تم الحصول علي عنوان المستخدم بنجاح"));
        }
        //[Authorize]
        //[HttpGet("user-by-email")]
        //GetUserByEmailAsync
        //GetUserIdByEmailAsync
        //we need this??????????????????? in future maybe for admin to search for user by email or for some other features but for now we can leave it out

    }
}
