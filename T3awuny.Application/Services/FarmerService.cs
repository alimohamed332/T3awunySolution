using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Address;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Core;
using T3awuny.Core.Entities.UserModule;
using T3awuny.Core.Specifications;

namespace T3awuny.Application.Services
{
    public class FarmerService : IFarmerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly string _baseUrl;

        public FarmerService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _baseUrl = configuration["App:ApplicationUrl"] ?? "";
        }

        public async Task<ApiResponse<IReadOnlyList<FarmerProfileDto>>> GetAllVerifiedAsync()
        {
            var farmerSpecification = new FarmerSpecifications(f => f.IsVerified);// I can use only base spec but will not include user and default add inside but it lighter if I didn't need it
            var farmerProfiles = await _unitOfWork.Repository<FarmerProfile>().GetAllWithSpecAsync(farmerSpecification);
            var mappedFarmerProfiles = farmerProfiles.Select(f => _mapper.Map<FarmerProfileDto>(f)).ToList();

            if (!mappedFarmerProfiles.Any())
                return ApiResponse<IReadOnlyList<FarmerProfileDto>>.Fail("لا يوجد مزارعين موثقين");

            foreach (var farmer in mappedFarmerProfiles)
            {
                if (!string.IsNullOrEmpty(farmer.ProfileImageUrl))
                {
                    farmer.ProfileImageUrl =$"{_baseUrl}{farmer.ProfileImageUrl}";
                }
            }
            return ApiResponse<IReadOnlyList<FarmerProfileDto>>.Ok(mappedFarmerProfiles, "تم العثور على المزارعين الموثقين بنجاح");
        }

        public async Task<FarmerProfileDto?> GetProfileAsync(string userId)
        {
            var farmerSpecification = new FarmerSpecifications(f => f.FarmerId == userId); //user
            var farmerProfile = await _unitOfWork.Repository<FarmerProfile>().GetByIdWithSpecAsync(farmerSpecification);
            if (farmerProfile is null) return null;
            var farmerDto = _mapper.Map<FarmerProfileDto>(farmerProfile);
            farmerDto.ProfileImageUrl = $"{_baseUrl}{farmerDto.ProfileImageUrl}";
            return farmerDto;
        }

        //public Task<FarmerProfileDto> GetPublicProfileAsync(int farmerId)
        //{
        //    throw new NotImplementedException();
        //}
        public async Task<FarmerProfileDto> CreateProfileAsync(string userId, CreateFarmerProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is  null) return new FarmerProfileDto { Messsage = "هذا المستخدم غير موجود" };
            //await _userManager.AddToRoleAsync(user, "Farmer"); we can move assign role here 

            if( !await _userManager.IsInRoleAsync(user, "Farmer")) return new FarmerProfileDto { Messsage = "هذا المستخدم ليس مزارع" };

            var existingProfile = await _unitOfWork.Repository<FarmerProfile>().GetByIdAsync(userId); 
            if (existingProfile is not null) return new FarmerProfileDto { Messsage = "هذا المستخدم لديه بروفايل بالفعل" };

            var farmerProfile = new FarmerProfile
            {
                FarmerId = userId,
                FarmName = dto.FarmName,
                Description = dto.Description,
                IsVerified = false
            };
            // Add the new farmer profile to the repository
            await _unitOfWork.Repository<FarmerProfile>().AddAsync(farmerProfile);
            await _unitOfWork.CompleteAsync();
            var farmerDto = _mapper.Map<FarmerProfileDto>(farmerProfile);
            //farmerDto.Name = user.Name;
            //farmerDto.Email = user.Email!;
            //farmerDto.UserName = user.UserName!;
            //farmerDto.JoinDate = user.JoinDate;
            var addSpecs = new BaseSpecifications<Address>(a => a.UserId == userId && a.IsDefault);
            var address = await _unitOfWork.Repository<Address>().GetByIdWithSpecAsync(addSpecs);
            farmerDto.Address = _mapper.Map<AddressDetailsDto>(address ?? new Address());
            farmerDto.ProfileImageUrl = $"{_baseUrl}{user.ProfileImageUrl}";
            return farmerDto;
        }

        public async Task<FarmerProfileDto> UpdateProfileAsync(string userId, UpdateFarmerProfileDto dto)
        {
            //var user = await _userManager.FindByIdAsync(userId);
            //if (user is null) return new FarmerProfileDto { Messsage = "هذا المستخدم غير موجود" };
            //await _userManager.AddToRoleAsync(user, "Farmer"); we can move assign role here  but the intial design خلاني مضطر 
            //if (!await _userManager.IsInRoleAsync(user, "Farmer")) return new FarmerProfileDto { Messsage = "هذا المستخدم ليس مزارع" };
            var profileSpecs = new FarmerSpecifications(fp => fp.FarmerId == userId); // user and default add
            var existingProfile = await _unitOfWork.Repository<FarmerProfile>().GetByIdWithSpecAsync(profileSpecs);
           
            if (existingProfile is null) return new FarmerProfileDto { Messsage = "هذا المزارع ليس لديه بروفايل" };
            if (existingProfile.User is null) return new FarmerProfileDto { Messsage = "هذا المستخدم غير موجود" };
            //if (!await _userManager.IsInRoleAsync(existingProfile.User, "Farmer")) return new FarmerProfileDto { Messsage = "هذا المستخدم ليس مزارع" };
            
            existingProfile.FarmName = string.IsNullOrEmpty(dto.FarmName) ? existingProfile.FarmName : dto.FarmName;
            existingProfile.Description = string.IsNullOrEmpty(dto.Description) ? existingProfile.Description : dto.Description;
            existingProfile.User!.Name = string.IsNullOrEmpty(dto.Name) ? existingProfile.User!.Name : dto.Name;
            await _unitOfWork.CompleteAsync();

            var farmerDto = _mapper.Map<FarmerProfileDto>(existingProfile);
            farmerDto.ProfileImageUrl = $"{_baseUrl}{farmerDto.ProfileImageUrl}";
            return farmerDto;
        }
    }
}
