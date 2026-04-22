using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.DTOs.Trader;
using T3awuny.Application.DTOs.User;
using T3awuny.Core;
using T3awuny.Core.Entities;
using T3awuny.Core.Repository.Contracts;
using T3awuny.Core.Specifications;

namespace T3awuny.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<FarmerProfileDto>>> GetPendingFarmersAsync()
        {
            var farmerSpecification = new FarmerSpecifications(f => !f.IsVerified);// you can use only base spec but will not include user inside but it lighter if you didn't need it
            var farmerProfiles = await _unitOfWork.Repository<FarmerProfile>().GetAllWithSpecAsync(farmerSpecification);
            var farmerProfilesDtos = farmerProfiles.Select(f => _mapper.Map<FarmerProfileDto>(f));

            if (!farmerProfilesDtos.Any())
                return ApiResponse<IEnumerable<FarmerProfileDto>>.Fail("لا يوجد حسابات مزارعين في انتظار التحقق حاليا");

            return ApiResponse<IEnumerable<FarmerProfileDto>>.Ok(farmerProfilesDtos, "تم العثور على حسابات المزارعين في انتظار التحقق بنجاح");
        }

        public async Task<ApiResponse<IEnumerable<TraderProfileDto>>> GetPendingTradersAsync()
        {
            var traderSpecification = new TraderSpecifications(t => !t.IsVerified);// you can use only base spec but will not include user inside but it lighter if you didn't need it
            var traderProfiles = await _unitOfWork.Repository<TraderProfile>().GetAllWithSpecAsync(traderSpecification);
            var traderProfilesDtos = traderProfiles.Select(t => _mapper.Map<TraderProfileDto>(t));

            if (!traderProfilesDtos.Any()) 
                return ApiResponse<IEnumerable<TraderProfileDto>>.Fail("لا يوجد حسابات تجار في انتظار التحقق حاليا");
            return ApiResponse<IEnumerable<TraderProfileDto>>.Ok(traderProfilesDtos, "تم العثور على حسابات التجار في انتظار التحقق بنجاح");
        }

        public async Task<ApiResponse<bool>> VerifyFarmerAsync(string farmerId)
        {
            var farmerSpecification = new FarmerSpecifications(f => f.FarmerId == farmerId);
            var farmerProfile = await _unitOfWork.Repository<FarmerProfile>().GetByIdWithSpecAsync(farmerSpecification);
            if (farmerProfile is null)
                return ApiResponse<bool>.Fail("هذا المزارع لا يملك بروفايل ");

            farmerProfile.IsVerified = true;
            farmerProfile.VerifiedAt = DateTime.UtcNow;
            farmerProfile!.User!.IsVerified = true;
            var result = _unitOfWork.Repository<FarmerProfile>().Update(farmerProfile);
            // you can use user manager to update the user but it will make 2 calls to the database so i prefer to update the user with the unit of work and then call complete async once to save all changes
            //await _userManager.UpdateAsync(farmerProfile.User);
            await _unitOfWork.CompleteAsync();
            if (!result.IsVerified)
                return ApiResponse<bool>.Fail("فشل في تحديث حالة المزارع حاول مرة اخرى لاحقاَ");

            return ApiResponse<bool>.Ok(true,"تم التحقق من المزارع بنجاح");
        }

        public async Task<ApiResponse<bool>> VerifyTraderAsync(string traderId)
        {
            var traderSpecification = new TraderSpecifications(t => t.TraderId == traderId); 
            var traderProfile = await _unitOfWork.Repository<TraderProfile>().GetByIdWithSpecAsync(traderSpecification);
            if (traderProfile is null)
                return ApiResponse<bool>.Fail("هذا التاجر لا يملك بروفايل ");

            traderProfile.IsVerified = true;
            traderProfile.VerifiedAt = DateTime.UtcNow;
            traderProfile!.User!.IsVerified = true;
            var result = _unitOfWork.Repository<TraderProfile>().Update(traderProfile);
            // you can use user manager to update the user but it will make 2 calls to the database so i prefer to update the user with the unit of work and then call complete async once to save all changes
            //await _userManager.UpdateAsync(traderProfile.User);
            await _unitOfWork.CompleteAsync();
            if (!result.IsVerified)
                return ApiResponse<bool>.Fail("فشل في تحديث حالة التاجر حاول مرة اخرى لاحقاَ");

            return ApiResponse<bool>.Ok(true, "تم التحقق من التاجر بنجاح");
        }

        public  async Task<ApiResponse<IEnumerable<BannedUserDto>>> GetBannedUsersAsync()
        {
            var bannedUsers = await _unitOfWork.UserRepository.GetBannedUsersAsync();
            if (!bannedUsers.Any())
            {
                return ApiResponse<IEnumerable<BannedUserDto>>.Fail("لا يوجد مستخدمين محظورين حاليا");
            }
            var bannedUsersDtos = bannedUsers.Select(u => new BannedUserDto 
            { 
                Id = u.Id,
                UserName = u.UserName!,
                Email = u.Email!,
                Name = u.Name
            });

            return ApiResponse<IEnumerable<BannedUserDto>>.Ok(bannedUsersDtos,"تم الحصول علي المستخدمين المحظورين بنجاح");
        }

        public async Task<ApiResponse<string>> ToggleUserStatusAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ApiResponse<string>.Fail("هذا المستخدم غير موجود");

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
                return ApiResponse<string>.Fail("لا تسطيع تغير حالة الحساب الخاص بمسؤول");

            user.IsActive = !user.IsActive;

            if (!user.IsActive)
                await RevokeAllRefreshTokensAsync(userId);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return ApiResponse<string>.Fail("فشل في تحديث حالة المستخدم حاول مرة اخرى لاحقاَ");

            var statusMessage = user.IsActive ? "تم تفعيل المستخدم" : "تم حظر المستخدم";
            return ApiResponse<string>.Ok($"تم {statusMessage} بنجاح");
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ApiResponse<bool>.Fail("هذا المستخدم غير موجود");
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
                return ApiResponse<bool>.Fail("لا تسطيع حذف الحساب الخاص بمسؤول");
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return ApiResponse<bool>.Fail("فشل في حذف المستخدم حاول مرة اخرى لاحقاَ");

            return ApiResponse<bool>.Ok(true, "تم حذف المستخدم بنجاح");
        }

        public async Task<ApiResponse<ApplicationUser>> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return ApiResponse<ApplicationUser>.Fail("هذا المستخدم غير موجود");
            return ApiResponse<ApplicationUser>.Ok(user, "تم الحصول علي المستخدم بنجاح");
        }

        public async Task<ApiResponse<ApplicationUser>> GetAdminByIdAsync(string adminId)
        {
            var user = await _userManager.FindByIdAsync(adminId);
            if (user is null)
                return ApiResponse<ApplicationUser>.Fail("هذا المسؤول غير موجود");
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
                return ApiResponse<ApplicationUser>.Fail("هذا المستخدم ليس مسؤول");

            return ApiResponse<ApplicationUser>.Ok(user, "تم الحصول علي المسؤول بنجاح");
        }

        private async Task RevokeAllRefreshTokensAsync(string userId)
        {
            var refreshTokenSpecObject = new BaseSpecifications<RefreshToken>(r => r.UserId == userId && r.IsActive);
            var token = await _unitOfWork.Repository<RefreshToken>()
                                          .GetByIdWithSpecAsync(refreshTokenSpecObject);
            if (token is not null)
            {
                token.RevokedOn = DateTime.UtcNow;

                await _unitOfWork.CompleteAsync();
            }
            
        }

    }
}
