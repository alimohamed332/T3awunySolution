using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs;
using T3awuny.Application.DTOs.User;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }  
        [Authorize]
        [HttpPut("update-profile-image")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateProfileImageAsync([FromForm] UpdateProfileImageDto dto)
        {
            if (dto.Image is null || dto.Image.Length == 0)
                return BadRequest(ApiResponse<string>.Fail("يرجى تحميل صورة صحيحة"));

            var result = await _userService.UpdateProfileImageAsync(dto.UserId, dto.Image);
            if (!result)
                return BadRequest(ApiResponse<string>.Fail("فشل تحديث صورة الملف الشخصي"));

            return Ok(ApiResponse<string>.Ok("", "تم تحديث صورة الملف الشخصي بنجاح"));
        }


        //[Authorize]
        //[HttpGet("user-by-email")]
        //GetUserByEmailAsync
        //GetUserIdByEmailAsync
        //we need this??????????????????? in future maybe for admin to search for user by email or for some other features but for now we can leave it out

    }
}
