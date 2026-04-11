using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Text;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Auth;
using T3awuny.Application.Services;
using T3awuny.Core.Entities;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<AuthModel>> RegisterAsync([FromBody] RegisterDto model)
        {
            var result = await _authService.RegisterAsync(model);
            if(!result.IsAuthenticated)
                return BadRequest(result);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthModel>> GetTokenAsync([FromBody] TokenRequestDto model)
        {
            var result = await _authService.GetTokenAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result);

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("add-role")]
        public async Task<ActionResult<string>> AddRoleAsync([FromBody] AddRoleDto model)
        {
            var result = await _authService.AddRoleAsync(model);
            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);
            return Ok();
        }

        [HttpGet("refresh-token")]
        public async Task<ActionResult<AuthModel>> RefreshTokenAsync(string? token) // if the frontend need to pass it in query string instead of the cookie
        {
            var refreshToken = token ?? Request.Cookies["refreshToken"];
            if(string.IsNullOrEmpty(refreshToken))
                return BadRequest("ابعت الريفرش توكن يابيه اما في الكوكي او في الكويري سترنح ");
            var result = await _authService.RefreshTokenAsync(refreshToken);
            if (!result.IsAuthenticated)
                return BadRequest(result);
            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            return Ok(result);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto model)
        {
            var refreshToken = model.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("ابعت الريفرش توكن يابيه اما في الكوكي او في الريكوست بودي");
            }
            var result = await _authService.RevokeTokenAsync(refreshToken);
            if (!result)
                return BadRequest("الريفرش توكن ده غير صالح");
            return Ok("خلاص عملنالك ريفوك");

        }

        [HttpGet("confirm-email")]
        public async Task<ActionResult<AuthModel>> ConfirmEmailAsync(string email, string token)
        {
            var model = new ConfirmEmailDto() { Email = email , ConfirmationToken = token };
            var result = await _authService.ConfirmEmailAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("send-confirmation-link")]
        public async Task<ActionResult<string>> SendConfirmationAsync(SendConfirmationLinkDto model)
        {
            var result = await _authService.SendConfirmationLinkAsync(model);
            if (!result)
                return BadRequest("هذا البريد الالكتروني غير مسجل مسبقاَ");
            return Ok("تم ارسال لينك لتأكيد بريدك الالكتروني ");
        }

        [Authorize]
        [HttpPost("forget-password")]
        public async Task<ActionResult<string>> ForgotPasswordAsync([FromBody] ForgotPasswordDto model)
        {
            var result = await _authService.ForgotPasswordAsync(model);
            if (!result)
                return BadRequest("هذا البريد الالكتروني غير مسجل مسبقاَ");
            return Ok("اضغط علي اللينك الذي سيصل عبر البريد الالكتروني");
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<AuthModel>> ResetPasswordAsync([FromBody] ResetPasswordDto model)
        {
            var result = await _authService.ResetPasswordAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("login-google")]
        public  IActionResult LoginGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<ActionResult<AuthModel>> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync("External");

            var claims = result?.Principal?.Identities
                .FirstOrDefault()?.Claims;

            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            await HttpContext.SignOutAsync("External");
            var loginDto = new LoginWithGoogleDto() { Email = email!,UserName = name!};
            var localResult = await _authService.LoginWithGoogleAsync(loginDto);

            if (!localResult.IsAuthenticated)
                return BadRequest(localResult);

            SetRefreshTokenInCookie(localResult.RefreshToken, localResult.RefreshTokenExpiration);

            return Ok(localResult);
        }
        private void SetRefreshTokenInCookie(string refreshToken, DateTime expiresOn)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expiresOn.ToLocalTime(),
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
