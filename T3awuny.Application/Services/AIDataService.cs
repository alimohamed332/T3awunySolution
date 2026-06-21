using AutoMapper;
using Microsoft.AspNetCore.Identity;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.AI_Dtos;
using T3awuny.Core;
using T3awuny.Core.Entities;
using T3awuny.Core.Entities.AuctionModule;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Entities.ReviewModule;
using T3awuny.Core.Entities.UserModule;
using T3awuny.Core.Repository.Contracts;
using T3awuny.Core.Specifications;
using T3awuny.Core.Specifications.AuctionSpecs;
using T3awuny.Core.Specifications.OrderSpecs;
using T3awuny.Core.Specifications.ReviewSpecs;

namespace T3awuny.Application.Services
{
    public class AIDataService : IAIDataService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public AIDataService(IUnitOfWork unitOfWork, IUserRepository userRepo, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userRepo = userRepo;
            _mapper = mapper;
            _userManager = userManager;
        }
    

        public async Task<IReadOnlyList<AIUserDto>> GetUsersData()
        {
            var users = await _userRepo.GetAIUsersDataForAIAsync();
            if (!users.Any())
                return new List<AIUserDto>();
            var usersDtos = new List<AIUserDto>();
            foreach (var user in users)
            {
                var userDto = _mapper.Map<AIUserDto>(user);

                if (user.FarmerProfile is not null)
                {
                    userDto.FarmerProfile = _mapper.Map<AIFarmerProfileDto>(user.FarmerProfile);
                    userDto.FarmerProfile.Governorate = user.Addresses.FirstOrDefault()?.Governorate ?? "";
                }               
                else if (user.TraderProfile is not null)
                    userDto.TraderProfile = _mapper.Map<AITraderProfileDto>(user.TraderProfile);

               var roles = await _userManager.GetRolesAsync(user);
               userDto.Role = roles.LastOrDefault() ?? "";
               
                usersDtos.Add(userDto);
            }

            return usersDtos;
        }

        public async Task<IReadOnlyList<AIReviewDto>> GetFarmerReviewsData()
        {
            var reviewSpecs = new ReviewSpecifications(r => true);
            var reviews = await _unitOfWork.Repository<Review>().GetAllWithSpecAsync(reviewSpecs);

            if (!reviews.Any())
                return new List<AIReviewDto>();

               return _mapper.Map<IReadOnlyList<AIReviewDto>>(reviews);
        }

        public async Task<IReadOnlyList<AIProductDto>> GetProductsData()
        {
            var products = await _unitOfWork.Repository<Product>().GetAllAsync();
            if (!products.Any())
                return new List<AIProductDto>();

            var proDtos = new List<AIProductDto>();
            foreach(var pro in products)
            {
                var proDto = _mapper.Map<AIProductDto>(pro);
                var addSpecs = new BaseSpecifications<Address>(ad => ad.UserId == pro.FarmerId);
                var address = await _unitOfWork.Repository<Address>().GetByIdWithSpecAsync(addSpecs);
                if (address is not null)
                    proDto.Governorate = address.Governorate;
                proDtos.Add(proDto);
            }
            return proDtos;
        }

        public async Task<IReadOnlyList<AIOrderDto>> GetOrdersData()
        {
            var orderSpec = new OrderSpecifications(o => true);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(orderSpec);
            if(!orders.Any())
                return new List<AIOrderDto>();

            var orderDtos = new List<AIOrderDto>();
            foreach (var order in orders)
            {
                var orderDto = _mapper.Map<AIOrderDto>(order);
                var addSpec = new BaseSpecifications<Address>(ad => ad.UserId == order.BuyerId);
                var address = await _unitOfWork.Repository<Address>().GetByIdWithSpecAsync(addSpec);
                if (address is not null)
                    orderDto.DeliveryAddress = _mapper.Map<AIOrderAddress>(address);
                orderDto.Payment = _mapper.Map<AIPaymentDto>(order.Payment);
                orderDto.Logistics = _mapper.Map<AILogisticsDto>(order.Logistics);
                orderDto.Items = order.Items.Select(oi => _mapper.Map<AIItemDto>(oi)).ToList();
                orderDtos.Add(orderDto);
            }
            return orderDtos;
        }

        public async Task<IReadOnlyList<AIBidDto>> GetBidsData()
        {
            var bidSpec = new BidSpecifications(b => true);
            var bids = await _unitOfWork.Repository<Bid>().GetAllWithSpecAsync(bidSpec);
            if (!bids.Any())
                return new List<AIBidDto>();

            return _mapper.Map<IReadOnlyList<AIBidDto>>(bids);
        }

        public async Task<IReadOnlyList<AIAuctionDto>> GetAuctionsData()
        {
            var auctionSpec = new AuctionSpecifications(a => true, verylighted: true);
            var auctions = await _unitOfWork.Repository<Auction>().GetAllWithSpecAsync(auctionSpec);
            if (!auctions.Any())
                return new List<AIAuctionDto>();

            return _mapper.Map<IReadOnlyList<AIAuctionDto>>(auctions);
        }
    }
}
