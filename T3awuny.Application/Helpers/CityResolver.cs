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
            return source?.Address?.City ?? source?.Address?.Town ?? source?.Address?.Village ?? "Unknown City";
        }
    }
    public class StreetResolver : IValueResolver<NominatimResponse, AddressDetailsDto, string>
    {
        public string Resolve(NominatimResponse source, AddressDetailsDto destination, string destMember, ResolutionContext context)
        {
            return source?.Address?.Road ?? "Unknown Street";
        }
    }
    public class GovernorateResolver : IValueResolver<NominatimResponse, AddressDetailsDto, string>
    {
        public string Resolve(NominatimResponse source, AddressDetailsDto destination, string destMember, ResolutionContext context)
        {
            return source?.Address?.State ?? "Unknown Governorate";
        }
    }
    public class CountryResolver : IValueResolver<NominatimResponse, AddressDetailsDto, string>
    {
        public string Resolve(NominatimResponse source, AddressDetailsDto destination, string destMember, ResolutionContext context)
        {
            return source?.Address?.Country ?? "Unknown Country";
        }
    }
    public class PostalCodeResolver : IValueResolver<NominatimResponse, AddressDetailsDto, string>
    {
        public string Resolve(NominatimResponse source, AddressDetailsDto destination, string destMember, ResolutionContext context)
        {
            return source?.Address?.PostCode ?? "Unknown Postal Code";
        }
    }

}
