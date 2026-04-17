using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Address;
using T3awuny.Core;
using T3awuny.Core.Entities;
using T3awuny.Core.Repository.Contracts;
using T3awuny.Core.Specifications;

namespace T3awuny.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGeocodingService _geocodingService;
        private readonly IMapper _mapper;

        public AddressService(IGeocodingService geocodingService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _geocodingService = geocodingService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<AddressDetailsDto?> GetAddressByIdAsync(int id)
        {
            var address = await _unitOfWork.Repository<Address>().GetByIdAsync(id); // no need to include => no need to use spec. no need to filter by userId since this is an admin endpoint
            if (address is null)
                return null;
            
            return new AddressDetailsDto
            {
                Id = address.Id,
                UserId = address.UserId,
                Street = address.Street!,
                City = address.City!,
                Governorate = address.Governorate!,
                PostalCode = address.PostalCode,
                Country = address.Country!,
                Latitude = address.Latitude,
                Longitude = address.Longitude
            };
        }

        public async Task<IEnumerable<AddressDetailsDto>> GetAllAddressesAsync()
        {
           var addresses = await _unitOfWork.Repository<Address>().GetAllAsync(); // no need to include => no need to use spec. no need to filter by userId since this is an admin endpoint
            return addresses.Select(address => new AddressDetailsDto
           {
               Id = address.Id,
               UserId = address.UserId,
               Street = address.Street!,
               City = address.City!,
               Governorate = address.Governorate!,
               PostalCode = address.PostalCode,
               Country = address.Country!,
               Longitude = address.Longitude,
               Latitude = address.Latitude
           });
        }
        public async Task<AddressDetailsDto> AddAddressAsync(string userId, CreateAddressDto dto)
        {
            // 1. Reverse geocode the coordinates
            var details = await _geocodingService.ReverseGeocodeAsync(dto.Latitude, dto.Longitude);
            // 2. Handle IsDefault logic
            if (dto.IsDefault)
            {
                var addressSpecificationObj = new AddressSpecifications(a => a.UserId == userId && a.IsDefault);
                var existing = await _unitOfWork.Repository<Address>().GetAllWithSpecAsync(addressSpecificationObj);
                foreach (var addr in existing)
                    addr.IsDefault = false;
            }

            var count = await _unitOfWork.Repository<Address>().GetCountAsync(new AddressSpecifications(a => a.UserId == userId));

            // 3. Build entity
            var address = new Address
            {
                UserId = userId,
                Label = dto.Label,
                Street = details.Street,
                City = details.City,
                Governorate = details.Governorate,
                PostalCode = details.PostalCode,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                IsDefault = dto.IsDefault || count == 0
            };

            await _unitOfWork.Repository<Address>().AddAsync(address);
            await _unitOfWork.CompleteAsync();
            var addressDetails = _mapper.Map<AddressDetailsDto>(address);
            return addressDetails;
        
        }   
    }
}
