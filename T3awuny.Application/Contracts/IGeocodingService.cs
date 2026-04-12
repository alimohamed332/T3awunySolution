using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.DTOs.Address;

namespace T3awuny.Application.Contracts
{
    public interface IGeocodingService
    {
        Task<AddressDetailsDto> ReverseGeocodeAsync(double latitude, double longitude);
    }
}
