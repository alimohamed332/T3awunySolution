using AutoMapper;
using T3awuny.Application.DTOs.Address;
using T3awuny.Application.DTOs.Admin;
using T3awuny.Application.DTOs.AI_Dtos;
using T3awuny.Application.DTOs.Auction;
using T3awuny.Application.DTOs.Auth;
using T3awuny.Application.DTOs.Category;
using T3awuny.Application.DTOs.Chat;
using T3awuny.Application.DTOs.DeliveryMethods;
using T3awuny.Application.DTOs.Farmer;
using T3awuny.Application.DTOs.Logistics;
using T3awuny.Application.DTOs.Order;
using T3awuny.Application.DTOs.Payment;
using T3awuny.Application.DTOs.Product;
using T3awuny.Application.DTOs.Review;
using T3awuny.Application.DTOs.Trader;
using T3awuny.Core.Entities;
using T3awuny.Core.Entities.AuctionModule;
using T3awuny.Core.Entities.ChatModule;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Entities.ReviewModule;
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
            CreateMap<Address, AddressDetailsDto>();

            CreateMap<FarmerProfile, FarmerProfileDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User!.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User!.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.UserName))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.User!.ProfileImageUrl))
                .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(src => src.User!.JoinDate))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User!.Addresses.FirstOrDefault()))
                .ForMember(dest => dest.Messsage, opt => opt.Ignore())
                ;

            CreateMap<TraderProfile,TraderProfileDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User!.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User!.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User!.UserName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.User!.Addresses.FirstOrDefault()))
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.MapFrom(src => src.User!.ProfileImageUrl))
                .ForMember(dest => dest.JoinDate, opt => opt.MapFrom(src => src.User!.JoinDate))
                .ForMember(dest => dest.Messsage, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ApplicationUser, AdminProfileDto>()
                .ForMember(dest => dest.Address, opt => opt.Ignore());
            CreateMap<Product, ProductSummaryDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.NameAr))
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => src.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl))
                .ForMember(dest => dest.FarmerCity, opt => opt.Ignore())
                .ForMember(dest => dest.FarmerGovernorate, opt => opt.Ignore())
                .ForMember(dest => dest.FarmerName, opt => opt.Ignore()) // I can get it from the internal farmer but in  i didn't catch it from DB in some functions for light retreive
                .ReverseMap();

            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.NameAr))
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => src.Images.FirstOrDefault(i => i.IsMain)!.ImageUrl))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(i => i.ImageUrl)))
                .ForMember(dest => dest.FarmerCity, opt => opt.Ignore()) 
                .ForMember(dest => dest.FarmerGovernorate, opt => opt.Ignore()) 
                .ForMember(dest => dest.FarmerImage, opt => opt.Ignore()) 
                .ForMember(dest => dest.FarmerName, opt => opt.Ignore()) // I can get it from the internal farmer but in  i didn't catch it from DB in some functions for light retreive
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

            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();

            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.BuyerName, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore())
                .ForMember(dest => dest.Logistics, opt => opt.Ignore())
                .ForMember(dest => dest.Payment, opt => opt.Ignore());

            CreateMap<Order, OrderSummaryDto>()
               .ForMember(dest => dest.BuyerName, opt => opt.MapFrom(src => src.Buyer.Name))
               .ForMember(dest => dest.Items, opt => opt.Ignore())
               .ForMember(dest => dest.LogisticsStatus, opt => opt.Ignore());

            CreateMap<OrderItem, OrderItemResponseDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ItemOrdered.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ItemOrdered.ProductName))
                .ForMember(dest => dest.MainImageUrl, opt => opt.MapFrom(src => src.ItemOrdered.PictureUrl))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.ItemOrdered.Unit));

            CreateMap<Logistics, LogisticsResponseDto>()
                .ForMember(dest => dest.PickupAddress, opt => opt.Ignore())
                .ForMember(dest => dest.DeliveryAddress, opt => opt.Ignore());

            CreateMap<Address, OrderAddress>();

            CreateMap<DeliveryMethod, DeliveryMethodResponseDto>();

            CreateMap<CreateDeliveryMethodDto, DeliveryMethod>();

            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.PayerName, opt => opt.Ignore());
            //.ForMember(dest => dest.PayerName, opt => opt.MapFrom(src => src.Payer.Name));
            /////////Auction
            CreateMap<CreateAuctionDto, Auction>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CurrentPrice, opt => opt.MapFrom(src => src.StartingPrice))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => AuctionStatus.Scheduled))
                .ForMember(dest => dest.FarmerId, opt => opt.Ignore());

            CreateMap<Auction, AuctionResponseDto>()
                .ForMember(dest => dest.TotalBids, opt => opt.MapFrom(src => src.Bids.Count()))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.Name))
                .ForMember(dest => dest.ProductUnit, opt => opt.MapFrom(src => src.Product!.Unit))
                .ForMember(dest => dest.ProductQuantity, opt => opt.MapFrom(src => src.Product!.Quantity))
                .ForMember(dest => dest.FarmerName, opt => opt.MapFrom(src => src.Farmer!.Name))
                .ForMember(dest => dest.FarmerImage, opt => opt.MapFrom(src => src.Farmer!.ProfileImageUrl??""))
                .ForMember(dest => dest.WinnerName, opt => opt.MapFrom(src => src.Winner!.Name??""))
                .ForMember(dest => dest.WinnerImage, opt => opt.MapFrom(src => src.Winner!.ProfileImageUrl??""))
                .ForMember(dest => dest.Bids, opt => opt.Ignore())
                .ForMember(dest => dest.MainImageUrl, opt => opt.Ignore());


            CreateMap<Auction, MyWinningtAuctionsDto>()
                //.ForMember(dest => dest.TotalBids, opt => opt.MapFrom(src => src.Bids.Count()))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.Name))
                .ForMember(dest => dest.ProductUnit, opt => opt.MapFrom(src => src.Product!.Unit))
                .ForMember(dest => dest.ProductQuantity, opt => opt.MapFrom(src => src.Product!.Quantity))
                .ForMember(dest => dest.FarmerName, opt => opt.MapFrom(src => src.Farmer!.Name))
                .ForMember(dest => dest.FarmerImage, opt => opt.MapFrom(src => src.Farmer!.ProfileImageUrl ?? ""))
                .ForMember(dest => dest.HighestBid, opt => opt.MapFrom(src => src.CurrentPrice))
                .ForMember(dest => dest.MainImageUrl, opt => opt.Ignore());

            CreateMap<Auction, AuctionResponseWithNoBidsDto>()
                .ForMember(dest => dest.TotalBids, opt => opt.MapFrom(src => src.Bids.Count()))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.Name))
                .ForMember(dest => dest.ProductUnit, opt => opt.MapFrom(src => src.Product!.Unit))
                .ForMember(dest => dest.ProductQuantity, opt => opt.MapFrom(src => src.Product!.Quantity))
                .ForMember(dest => dest.FarmerName, opt => opt.MapFrom(src => src.Farmer!.Name))
                .ForMember(dest => dest.FarmerImage, opt => opt.MapFrom(src => src.Farmer!.ProfileImageUrl ?? ""))
                .ForMember(dest => dest.WinnerName, opt => opt.MapFrom(src => src.Winner!.Name ?? ""))
                .ForMember(dest => dest.WinnerImage, opt => opt.MapFrom(src => src.Winner!.ProfileImageUrl ?? ""))
                .ForMember(dest => dest.MainImageUrl, opt => opt.Ignore());

            CreateMap<Bid, BidResponseDto>()
            .ForMember(dest => dest.BidderName, opt => opt.Ignore())
            .ForMember(dest => dest.BidderImage, opt => opt.Ignore());

            CreateMap<Auction, AuctionSummaryDto>();

            //Review
            CreateMap<Review,ReviewResponseDto>()
                .ForMember(dest => dest.TargetName, opt => opt.MapFrom(src => src.TargetUser!.Name ?? ""))             
                .ForMember(dest => dest.TargetImageUrl, opt => opt.MapFrom(src => src.TargetUser!.ProfileImageUrl ?? ""))             
                .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.Reviewer!.Name ?? ""))             
                .ForMember(dest => dest.ReviewerImageUrl, opt => opt.MapFrom(src => src.Reviewer!.ProfileImageUrl ?? ""))             
                ;

            //Chat
            CreateMap<Message, MessageResponseDto>()
                .ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.SentAt.AddHours(3))) ///////////////////////////////////////////////////////
                .ForMember(dest => dest.ReceiverId, opt => opt.Ignore());

            CreateMap<ApplicationUser,AdminUserDto>()
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.TotalOrders, opt => opt.Ignore())
                .ForMember(dest => dest.TotalProducts, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());
            //=============================AI=============================
            CreateMap<ApplicationUser,AIUserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.FarmerProfile, opt => opt.Ignore())
                .ForMember(dest => dest.TraderProfile, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<FarmerProfile, AIFarmerProfileDto>()
                .ForMember(dest => dest.Governorate, opt => opt.Ignore());

            CreateMap<TraderProfile, AITraderProfileDto>()
                .ForMember(dest => dest.BusinessType, opt => opt.MapFrom(src => src.BusinessType!.Value.ToString()));

            CreateMap<Review,AIReviewDto>()
                .ForMember(dest => dest.ReviewerName, opt => opt.MapFrom(src => src.Reviewer.Name))
                .ForMember(dest => dest.TargetFarmerId, opt => opt.MapFrom(src => src.TargetUserId))
                .ForMember(dest => dest.TargetFarmerName, opt => opt.MapFrom(src => src.TargetUser.Name));

            CreateMap<Product,AIProductDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Governorate, opt => opt.Ignore());

            CreateMap<Address, AIOrderAddress>();

            CreateMap<Order,AIOrderDto>()
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.BuyerName, opt => opt.MapFrom(src => src.Buyer.Name))
                .ForMember(dest => dest.DeliveryAddress, opt => opt.Ignore());

            CreateMap<Logistics, AILogisticsDto>();
            CreateMap<Payment, AIPaymentDto>();
            CreateMap<OrderItem, AIItemDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ItemOrdered.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ItemOrdered.ProductName));

            CreateMap<Bid, AIBidDto>()
                .ForMember(dest => dest.BidderName, opt => opt.MapFrom(src => src.Bidder.Name));

            CreateMap<Auction,AIAuctionDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.Name))
                .ForMember(dest => dest.FarmerName, opt => opt.MapFrom(src => src.Farmer!.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        }
    }
}