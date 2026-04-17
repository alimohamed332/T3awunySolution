using AutoMapper;
using AutoMapper.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.DTOs.Address;
using T3awuny.Application.DTOs.Auth;
using T3awuny.Core.Entities;

namespace T3awuny.Application.Helpers
{
    public class CityResolver : IValueResolver<NominatimResponse, AddressDetailsDto, string>
    {
        public string Resolve(NominatimResponse source, AddressDetailsDto destination, string destMember, ResolutionContext context)
        {
            return source?.Address?.City ?? source?.Address?.Town ?? source?.Address?.Village ?? "الفيوم";
        }
    }
    public class StreetResolver : IValueResolver<NominatimResponse, AddressDetailsDto, string>
    {
        public string Resolve(NominatimResponse source, AddressDetailsDto destination, string destMember, ResolutionContext context)
        {
            return source?.Address?.Road ?? "شارع غير معروف";
        }
    }
    public class GovernorateResolver : IValueResolver<NominatimResponse, AddressDetailsDto, string>
    {
        public string Resolve(NominatimResponse source, AddressDetailsDto destination, string destMember, ResolutionContext context)
        {
            return source?.Address?.State ?? "محافظة الفيوم";
        }
    }
    public class CountryResolver : IValueResolver<NominatimResponse, AddressDetailsDto, string>
    {
        public string Resolve(NominatimResponse source, AddressDetailsDto destination, string destMember, ResolutionContext context)
        {
            return source?.Address?.Country ?? "مصر";
        }
    }
    public class PostalCodeResolver : IValueResolver<NominatimResponse, AddressDetailsDto, int?>
    {
        public int? Resolve(NominatimResponse source, AddressDetailsDto destination, int? destMember, ResolutionContext context)
        {
            return int.TryParse(source?.Address?.PostCode, out int result) ? result : null;
        }
    }

}
