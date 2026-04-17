using AutoMapper;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

//using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
//using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Auth;
using T3awuny.Application.JwtFeatures;
using T3awuny.Core.Entities;

namespace T3awuny.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly JwtHandler _jwtHandler;
        private readonly RefreshTokenHandler _refreshTokenHandler;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IAddressService _addressService;

        public AuthService(UserManager<ApplicationUser> userManager, JwtHandler jwtHandler, IMapper mapper, RoleManager<IdentityRole> roleManager, RefreshTokenHandler refreshTokenHandler, IConfiguration configuration, IEmailService emailService, IFileStorageService fileStorageService, IAddressService addressService)
        {
            _userManager = userManager;
            _jwtHandler = jwtHandler;
            _mapper = mapper;
            _roleManager = roleManager;
            _refreshTokenHandler = refreshTokenHandler;
            _configuration = configuration;
            _emailService = emailService;
            _fileStorageService = fileStorageService;
            _addressService = addressService;
        }

        public async Task<AuthModel> RegisterAsync(RegisterDto model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = "هذا البريد الالكتروني مسجل بالفعل " };

            if (await _userManager.FindByNameAsync(model.UserName) is not null)
                return new AuthModel { Message = "اسم المستخدم هذا مسجل من قبل" };
            var user = _mapper.Map<ApplicationUser>(model);

            try
            {
                user.ProfileImageUrl = await _fileStorageService.SaveImageAsync(model.ImageFile, "users");
            }
            catch (Exception) { }

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                StringBuilder errors = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    errors.Append(error.Description + " , ");
                }
                return new AuthModel { Message = errors.ToString() };
            }
            
            //foreach(var add in  model.Addresses)
            await _addressService.AddAddressAsync(user.Id, model.Addresses);//add);

            var role = model.Role?.ToLower().Contains("ta") == true ? "Trader" : "Farmer";

            await _userManager.AddToRoleAsync(user, role);

            List<string> roles = new List<string>();
            roles.Add(role); // no loop because we have only one role for each user in this case 
            var token = _jwtHandler.CreateToken(user, roles);

            // generate email confirmation token
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(confirmationToken));

            //var encodedToken = WebUtility.UrlEncode(confirmationToken);

            var confirmationLink =
                $"{_configuration["App:ApplicationUrl"]}/api/Auth/confirm-email?email={user.Email}&token={encodedToken}";

            await _emailService.SendAsync(
                user.Email!,
                "تأكيد بريدك الالكتروني",
                $"اضغط علي اللينك لتأكيد بريدك الالكتروني: {confirmationLink}");

            return new AuthModel
            {
                Email = user.Email!,
                Username = user.UserName!,
                IsAuthenticated = true,
                Token = token,
                Roles = roles,
                Message = "تم التسجيل بنجاح برجاء تأكيد البريد الالكتروني لتتمكن من استخدام حسابك"
            };

        }

        public async Task<AuthModel> GetTokenAsync(TokenRequestDto model)
        {
            var authModel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "البريد الالكتروني او كلمة السر غير صحيحة ";
                return authModel;
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                authModel.Message = "لم يتم تأكيد البريد الالكتروني اطلب إعادة ارسال لينك تأكيد مرة اخري";
                return authModel;
            }
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtHandler.CreateToken(user, roles);
            authModel.Email = user.Email ?? "";
            authModel.IsAuthenticated = true;
            authModel.Username = user.UserName ?? "";
            authModel.Roles = roles.ToList();
            authModel.Token = token;
            authModel.Message = "نم تسجيل الدخول بنجاح";

            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken?.Token!;
                authModel.RefreshTokenExpiration = activeRefreshToken!.ExpiresOn;
            }
            else
            {
                var refreshToken = _refreshTokenHandler.GenerateRefreshToken();            
                authModel.RefreshToken = refreshToken?.Token!;
                authModel.RefreshTokenExpiration = refreshToken!.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }
            return authModel;
        }

        public async Task<string> AddRoleAsync(AddRoleDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null || !await _roleManager.RoleExistsAsync(model.Role))
                return "هناك مشكلة في رقم المستخدم او ان هذا الدور غير مدعوم";
            if (await _userManager.IsInRoleAsync(user, model.Role))
                return "هذا المستخدم موجود في هذه المسئولية بالفعل";
            var result = await _userManager.AddToRoleAsync(user, model.Role);
            return result.Succeeded ? string.Empty : "حدث شئ ما خطأ";
        }

        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var authModel = new AuthModel();
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user is null)
            {
                authModel.Message = "!الريفرش توكن غير صالح";
                return authModel;
            }
            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
            if (!refreshToken.IsActive)
            {
                authModel.Message = "الريفرش توكن منتهي الصلاحية";
                return authModel;
            }
            //Revoke and generate new refresh and access token
            refreshToken.RevokedOn = DateTime.UtcNow;
            var newRefreshToken = _refreshTokenHandler.GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);
            //new access token
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtHandler.CreateToken(user, roles);

            authModel.IsAuthenticated = true;
            authModel.Email = user.Email!;
            authModel.Username = user.UserName!;
            authModel.Roles = roles.ToList();
            authModel.Token = accessToken;
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModel;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user is null) 
                return false;
            var refreshToken = user.RefreshTokens.Single(t =>  t.Token == token);
            if (!refreshToken.IsActive)
                return false;
            refreshToken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return true;
        }

        public async Task<AuthModel> ConfirmEmailAsync(ConfirmEmailDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return new AuthModel() { Message = "هذا البريد الالكتروني غي مسجل من قبل" };

            //var decodedToken = WebUtility.UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(Base64UrlEncoder.DecodeBytes(model.ConfirmationToken));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                return new AuthModel() { Message = "هذا التوكن غير صحيح" };
            var roles = (List<string>)await _userManager.GetRolesAsync(user);
            return new AuthModel
            {
                IsAuthenticated = true,
                Email = user.Email!,
                Username = user.UserName!,
                Message = "تم تأكيد البريد الالكتروني",
                Roles = roles

            };
        }

        public async Task<bool> SendConfirmationLinkAsync(SendConfirmationLinkDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return false;
            // generate email confirmation token
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Base64UrlEncoder.Encode(Encoding.UTF8.GetBytes(confirmationToken));

            //var encodedToken = WebUtility.UrlEncode(confirmationToken);

            var confirmationLink =
                $"{_configuration["App:ApplicationUrl"]}/api/Auth/confirm-email?email={user.Email}&token={encodedToken}";

            await _emailService.SendAsync(
                user.Email!,
                "تأكيد بريدك الالكتروني",
                $"اضغط علي اللينك لتأكيد بريدك الالكتروني: {confirmationLink}");
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
                return false;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebUtility.UrlEncode(token);

            var resetLink =
                $"{_configuration["App:FrontendUrl"]}/reset-password?email={model.Email}&token={encodedToken}";

            await _emailService.SendAsync(
                model.Email,
                "تغيير كلمة المرور",
                $"غير كلمة المرور ياستخدام هذا الرابط: {resetLink}");

            return true;
        }

        public async Task<AuthModel> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
                return new AuthModel() { Message = "هذا البريد الالكتروني غير مسجل مسبقاَ" };

            var decodedToken = WebUtility.UrlDecode(model.Token);

            var result = await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                    errors.Add(error.Description);
                return new AuthModel() { Message = errors.ToString()!  };
            }
            return new AuthModel()
            {
                IsAuthenticated = true,
                Email = user.Email!,
                Username = user.UserName!,
                Message = "نم تغير كلمة المرور بنجاح"
            };

        }

        public async Task<AuthModel> LoginWithGoogleAsync(LoginWithGoogleDto model)
        {
            var authModel = new AuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null) // register its email 
            {
                var appUser = new ApplicationUser() { Email = model.Email, Name = model.UserName, UserName = model.UserName.Replace(" ","") };
                var result = await _userManager.CreateAsync(appUser);
                if (!result.Succeeded)
                {
                    var errors = new List<string>();
                    foreach (var error in result.Errors)
                        errors.Add(error.Description);
                    authModel.Message = errors.ToString()!;
                    return authModel;
                }
                user = await _userManager.FindByEmailAsync(model.Email);
            }            
            //generate access and refresh token
            var roles = (List<string>)await _userManager.GetRolesAsync(user!);
            var token = _jwtHandler.CreateToken(user!, roles);

            if (user!.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken?.Token!;
                authModel.RefreshTokenExpiration = activeRefreshToken!.ExpiresOn;
            }
            else
            {
                var refreshToken = _refreshTokenHandler.GenerateRefreshToken();
                authModel.RefreshToken = refreshToken?.Token!;
                authModel.RefreshTokenExpiration = refreshToken!.ExpiresOn;
                user?.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user!);
            }
            authModel.IsAuthenticated = true;
            authModel.Message = "تم تسجيل الدخول باستخدام جوجل بنجاح";
            authModel.Email = user!.Email!;
            authModel.Username = user.UserName!;
            authModel.Token = token;
            authModel.Roles = roles;

            return authModel;                  
        }
    }
}
