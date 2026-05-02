using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.DTOs.Address;
using T3awuny.Application.DTOs.Auth;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.DTOs.Order;
using T3awuny.Application.DTOs.Product;
using T3awuny.Application.DTOs.Trader;
using T3awuny.Core.Entities;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Entities.UserModule;

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
                .ForMember(dest => dest.Addresses, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<NominatimResponse, AddressDetailsDto>()
                .ForMember(dest => dest.Street, opt => opt.MapFrom<StreetResolver>())
                .ForMember(dest => dest.City, opt => opt.MapFrom<CityResolver>())
                .ForMember(dest => dest.Governorate, opt => opt.MapFrom<GovernorateResolver>())
                .ForMember(dest => dest.PostalCode, opt => opt.MapFrom<PostalCodeResolver>())
                .ForMember(dest => dest.Country, opt => opt.MapFrom<CountryResolver>())
                .ReverseMap();
            CreateMap<Address, AddressDetailsDto>()
                .ReverseMap();

            CreateMap<FarmerProfile, FarmerProfileDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User!.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User!.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.UserName))
                .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.User!.Addresses))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.User!.ProfileImageUrl))
                .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(src => src.User!.JoinDate))
                .ForMember(dest => dest.Messsage, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<TraderProfile,TraderProfileDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User!.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User!.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.UserName))
                .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.User!.Addresses))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.User!.ProfileImageUrl))
                .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(src => src.User!.JoinDate))
                .ForMember(dest => dest.Messsage, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Product, ProductSummaryDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.NameAr))
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => src.Images.FirstOrDefault()!.ImageUrl))
                .ForMember(dest => dest.FarmerName, opt => opt.MapFrom(src => src.Farmer.Name))
                .ReverseMap();

            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.NameAr))
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => src.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(i => i.ImageUrl)))
                .ForMember(dest => dest.FarmerName, opt => opt.MapFrom(src => src.Farmer.Name))
                .ForMember(dest => dest.FarmerGovernorate, opt => opt.MapFrom(src => src.Farmer.Addresses.FirstOrDefault(a => a.IsDefault)!.Governorate))
                .ReverseMap();

            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.FarmerId, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.Farmer, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.PublishImmediately ? ProductStatus.Active : ProductStatus.Draft));


            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom<ProductNameResolver>())
                .ForMember(dest => dest.Unit, opt => opt.MapFrom<ProductUnitResolver>())
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom<ProductUnitPriceResolver>())
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom<ProductQuantityResolver>())
                .ForMember(dest => dest.Description, opt => opt.MapFrom<ProductDescriptionResolver>())
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom<ProductCategoryIdtResolver>())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Farmer, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore());
                ;
            //.ForAllMembers(opt => opt.Condition((src,dest,srcMember) => srcMember is not null && (!(srcMember is string
            //str) || !string.IsNullOrEmpty(str)) ));

            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.BuyerName, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore())
                .ForMember(dest => dest.Logistics, opt => opt.Ignore());

            CreateMap<Order, OrderSummaryDto>()
               .ForMember(dest => dest.BuyerName, opt => opt.Ignore())
               .ForMember(dest => dest.Items, opt => opt.Ignore())
               .ForMember(dest => dest.LogisticsStatus, opt => opt.Ignore());
            CreateMap<OrderItem, OrderItemResponseDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ItemOrdered.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ItemOrdered.ProductName))
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => src.ItemOrdered.PictureUrl))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.ItemOrdered.Unit));
        }
    }
}