using Microsoft.AspNetCore.Identity;
//using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.DTOs.Auth;
using T3awuny.Core.Entities;

namespace T3awuny.Application.Contracts
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterDto model);
        Task<AuthModel> GetTokenAsync(TokenRequestDto model);
        Task<string> AddRoleAsync(AddRoleDto model);
        Task<AuthModel> RefreshTokenAsync(string token); // this the refresh token
        Task<bool> RevokeTokenAsync(string token);
        Task<AuthModel> ConfirmEmailAsync(ConfirmEmailDto model);
        Task<bool> SendConfirmationLinkAsync(SendConfirmationLinkDto model);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto model);
        Task<AuthModel> ResetPasswordAsync(ResetPasswordDto model);
        Task<AuthModel> LoginWithGoogleAsync(LoginWithGoogleDto model);
    }
}
