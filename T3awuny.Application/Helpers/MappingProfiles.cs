using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.DTOs.Address;
using T3awuny.Application.DTOs.Auth;
using T3awuny.Core.Entities;

namespace T3awuny.Application.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName))
                //.ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.ProfileImageUrl))
                .ReverseMap();

            CreateMap<NominatimResponse, AddressDetailsDto>()
                .ForMember(dest => dest.Street, opt => opt.MapFrom<StreetResolver>())
                .ForMember(dest => dest.City, opt => opt.MapFrom<CityResolver>())
                .ForMember(dest => dest.Governorate, opt => opt.MapFrom<GovernorateResolver>())
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom<PostalCodeResolver>())
                .ForMember(dest => dest.Country, opt => opt.MapFrom<CountryResolver>())
                .ReverseMap();
        }
    }
}