using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Address;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.DTOs.Trader;
using T3awuny.Core;
using T3awuny.Core.Entities.UserModule;
using T3awuny.Core.Specifications;

namespace T3awuny.Application.Services
{
    public class TraderService : ITraderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly string _baseUrl;
        public TraderService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _baseUrl = configuration["App:ApplicationUrl"] ?? "";
        }

        public async Task<ApiResponse<IReadOnlyList<TraderProfileDto>>> GetAllVerifiedAsync()
        {
            var traderSpecification = new TraderSpecifications(t => t.IsVerified); //user and default address 
            var traderProfiles = await _unitOfWork.Repository<TraderProfile>().GetAllWithSpecAsync(traderSpecification);
            var mappedTraderProfiles = traderProfiles.Select(t => _mapper.Map<TraderProfileDto>(t)).ToList();

            if (!mappedTraderProfiles.Any())
                return ApiResponse<IReadOnlyList<TraderProfileDto>>.Fail("لا يوجد تجار موثقين");
            foreach (var trader in mappedTraderProfiles)
            {
                if (!string.IsNullOrEmpty(trader.ProfileImageUrl))
                {
                    trader.ProfileImageUrl =$"{_baseUrl}{trader.ProfileImageUrl}";
                }
            }

            return ApiResponse<IReadOnlyList<TraderProfileDto>>.Ok(mappedTraderProfiles, "تم العثور على التجار الموثقين بنجاح");
        } 

        public async Task<TraderProfileDto?> GetProfileAsync(string userId)
        {
            var traderSpecification = new TraderSpecifications(t => t.TraderId == userId); //user and default address
            var traderProfile = await _unitOfWork.Repository<TraderProfile>().GetByIdWithSpecAsync(traderSpecification);
            if (traderProfile is null) return null;
            var traderDto = _mapper.Map<TraderProfileDto>(traderProfile);
            traderDto.ProfileImageUrl = $"{_baseUrl}{traderDto.ProfileImageUrl}";
            return traderDto;
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

            var traderDto = _mapper.Map<TraderProfileDto>(traderProfile);
            var addSpecs = new BaseSpecifications<Address>(a => a.UserId == userId && a.IsDefault);
            var address = await _unitOfWork.Repository<Address>().GetByIdWithSpecAsync(addSpecs);
            traderDto.Address = _mapper.Map<AddressDetailsDto>(address ?? new Address());
            traderDto.ProfileImageUrl = $"{_baseUrl}{traderDto.ProfileImageUrl}";
            return traderDto;
        }
 
        public async Task<TraderProfileDto> UpdateProfileAsync(string userId, UpdateTraderProfileDto dto)
        {
            var profileSpecs = new TraderSpecifications(tp => tp.TraderId == userId); // user and default add
            var existingProfile = await _unitOfWork.Repository<TraderProfile>().GetByIdWithSpecAsync(profileSpecs);

            if (existingProfile is null) return new TraderProfileDto { Messsage = "هذا التاجر ليس لديه بروفايل" };
            if (existingProfile.User is null) return new TraderProfileDto { Messsage = "هذا المستخدم غير موجود" };
            if (!await _userManager.IsInRoleAsync(existingProfile.User, "Trader")) return new TraderProfileDto { Messsage = "هذا المستخدم ليس تاجر" };
           
            existingProfile.BusinessName = dto.BusinessName ?? existingProfile.BusinessName;
            existingProfile.BusinessType = dto.BusinessType;
            existingProfile.Description = dto.Description ?? existingProfile.Description;
            existingProfile.User!.Name = dto.Name ?? existingProfile.User!.Name;
            await _unitOfWork.CompleteAsync();

            var traderDto = _mapper.Map<TraderProfileDto>(existingProfile);
            traderDto.ProfileImageUrl = $"{_baseUrl}{traderDto.ProfileImageUrl}";
            return traderDto;
        }
    }
}
