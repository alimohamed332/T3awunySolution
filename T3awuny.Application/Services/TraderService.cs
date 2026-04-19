using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.DTOs.Trader;
using T3awuny.Core;
using T3awuny.Core.Entities;
using T3awuny.Core.Specifications;

namespace T3awuny.Application.Services
{
    public class TraderService : ITraderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public TraderService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IEnumerable<TraderProfileDto>> GetAllVerifiedAsync()
        {
            var traderSpecification = new TraderSpecifications(t => t.IsVerified); // you can use only base spec but will not include user inside but it lighter if you didn't need it 
            var traderProfiles = await _unitOfWork.Repository<TraderProfile>().GetAllWithSpecAsync(traderSpecification);
            return traderProfiles.Select(t => _mapper.Map<TraderProfileDto>(t)); // if ok do it in farmer service
        }

        public async Task<TraderProfileDto?> GetProfileAsync(string userId)
        {
            var traderSpecification = new TraderSpecifications(t => t.TraderId == userId);
            var traderProfile = await _unitOfWork.Repository<TraderProfile>().GetByIdWithSpecAsync(traderSpecification);
            if (traderProfile is null) return null;
            return _mapper.Map<TraderProfileDto>(traderProfile);
        }
        public async Task<TraderProfileDto> CreateProfileAsync(string userId, CreateTraderProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return new TraderProfileDto { Messsage = "هذا المستخدم غير موجود" };
            //await _userManager.AddToRoleAsync(user, "Trader"); we can move assign role here 

            if (!await _userManager.IsInRoleAsync(user, "Trader")) return new TraderProfileDto { Messsage = "هذا المستخدم ليس تاجر" };

            var existingProfile = await _unitOfWork.Repository<TraderProfile>().GetByIdAsync(userId);
            if (existingProfile is not null) return new TraderProfileDto { Messsage = "هذا المستخدم لديه بروفايل بالفعل" };

            var traderProfile = new TraderProfile
            {
                TraderId = userId,
                BusinessName = dto.BusinessName,
                BusinessType = dto.BusinessType,
                Description = dto.Description,
                TaxNumber = dto.TaxNumber,
                IsVerified = false
            };
            // Add the new trader profile to the repository
            await _unitOfWork.Repository<TraderProfile>().AddAsync(traderProfile);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<TraderProfileDto>(traderProfile);
        }
 
        public async Task<TraderProfileDto> UpdateProfileAsync(string userId, UpdateTraderProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return new TraderProfileDto { Messsage = "هذا المستخدم غير موجود" };
            //await _userManager.AddToRoleAsync(user, "Trader"); we can move assign role here 
            if (!await _userManager.IsInRoleAsync(user, "Trader")) return new TraderProfileDto { Messsage = "هذا المستخدم ليس تاجر" };

            var existingProfile = await _unitOfWork.Repository<TraderProfile>().GetByIdAsync(userId);
            if (existingProfile is null) return new TraderProfileDto { Messsage = "هذا التاجر ليس لديه بروفايل" };

            existingProfile.BusinessName = dto.BusinessName ?? existingProfile.BusinessName;
            existingProfile.BusinessType = dto.BusinessType;
            existingProfile.Description = dto.Description ?? existingProfile.Description;
            existingProfile.User!.Name = dto.Name ?? existingProfile.User!.Name;
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<TraderProfileDto>(existingProfile);
        }
    }
}
