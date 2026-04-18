using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Address;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Core;
using T3awuny.Core.Entities;
using T3awuny.Core.Specifications;

namespace T3awuny.Application.Services
{
    public class FarmerService : IFarmerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public FarmerService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IEnumerable<FarmerProfileDto>> GetAllVerifiedAsync()
        {
            var farmerSpecification = new FarmerSpecifications(f => f.IsVerified);
            var farmerProfiles = await _unitOfWork.Repository<FarmerProfile>().GetAllWithSpecAsync(farmerSpecification);
            return farmerProfiles.Select(f => new FarmerProfileDto
            {
                FarmerId = f.FarmerId,
                Name = f.User!.Name,
                FarmName = f.FarmName,
                Description = f.Description,
                Email = f.User!.Email!,
                UserName = f.User!.UserName!,
                ProfileImageUrl = f.User!.ProfileImageUrl!,
                JoinDate = f.User!.JoinDate,
                IsVerified = f.IsVerified
            });
        }

        public async Task<FarmerProfileDto?> GetProfileAsync(string userId)
        {
            var farmerSpecification = new FarmerSpecifications(f => f.FarmerId == userId);
            var farmerProfile = await _unitOfWork.Repository<FarmerProfile>().GetByIdWithSpecAsync(farmerSpecification);
            if (farmerProfile is null) return null;
            return _mapper.Map<FarmerProfileDto>(farmerProfile);
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
            return _mapper.Map<FarmerProfileDto>(farmerProfile);
        }

        public async Task<FarmerProfileDto> UpdateProfileAsync(string userId, UpdateFarmerProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return new FarmerProfileDto { Messsage = "هذا المستخدم غير موجود" };
            //await _userManager.AddToRoleAsync(user, "Farmer"); we can move assign role here 

            if (!await _userManager.IsInRoleAsync(user, "Farmer")) return new FarmerProfileDto { Messsage = "هذا المستخدم ليس مزارع" };

            var existingProfile = await _unitOfWork.Repository<FarmerProfile>().GetByIdAsync(userId);
            if (existingProfile is null) return new FarmerProfileDto { Messsage = "هذا المستخدم ليس لديه بروفايل" };

            existingProfile.FarmName = dto.FarmName;
            existingProfile.Description = dto.Description;
            existingProfile.User!.Name = dto.Name;
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<FarmerProfileDto>(existingProfile);
        }
    }
}
