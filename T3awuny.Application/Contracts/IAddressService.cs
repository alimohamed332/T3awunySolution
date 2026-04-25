using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.DTOs.Address;

namespace T3awuny.Application.Contracts
{
    public interface IAddressService
    {
        Task<IReadOnlyList<AddressDetailsDto>> GetAllAddressesAsync();
        Task<AddressDetailsDto?> GetAddressByIdAsync(int id);
        Task<AddressDetailsDto> AddAddressAsync(string userId, CreateAddressDto dto);
        //Task<AddressDetailsDto?> UpdateAddressAsync(int id, AddressDetailsDto addressDto);
        //Task<bool> DeleteAddressAsync(int id);
    }
}
