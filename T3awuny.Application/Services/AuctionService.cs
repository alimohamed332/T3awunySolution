using AutoMapper;
using Microsoft.Extensions.Configuration;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Auction;
using T3awuny.Application.Helpers;
using T3awuny.Core;
using T3awuny.Core.Entities;
using T3awuny.Core.Entities.AuctionModule;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Entities.ProductModule;
using T3awuny.Core.Entities.UserModule;
using T3awuny.Core.Specifications;
using T3awuny.Core.Specifications.AuctionSpecs;

namespace T3awuny.Application.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IPaymentService _paymentService;
        private readonly string _baseUrl;
        //private readonly IHubContext<AuctionHub> _hubContext;  // SignalR
        public AuctionService(IUnitOfWork unitOfWork/*, IHubContext<AuctionHub> hubContext*/, IMapper mapper, IEmailService emailService, IPaymentService paymentService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _paymentService = paymentService;
            _baseUrl = configuration["App:ApplicationUrl"]??"";
        }
        // HasActiveAcution in product model false => in ProcessAuctionEndsAsync and CancelAuctionAsync and true only in CreateAuctionAsync
        // ده عشان اللي انا عايزه دلوقتي ان اعرف اذا كان المنتج عليه مزاد او هيبقي عليه ولا لا انما لو غيرت اللوجيك لهل عليه (حاليا) اي مزاد اكتف ولا لا ساعتها
        // HasActiveAcution in product model false => in ProcessAuctionEndsAsync and CancelAuctionAsync and true only in ProcessAuctionStartsAsync
        public async Task<ApiResponse<Auction>> CreateAuctionAsync(string farmerId, CreateAuctionDto dto)
        {
            //validate farmer is active => token no but refresh token yes
            // 1. Validate product ownership
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(dto.ProductId);
            if (product is null || product.FarmerId != farmerId)
                return ApiResponse<Auction>.Fail("لم نجد منتج مملوك لهذا المزارع بذالك المعرف");

            if (product.Status != ProductStatus.Active)
                return ApiResponse<Auction>.Fail("لايمكن ان تنشئ مزاد علي منتج في هذه الحالة");

            // 2. Check no existing active/scheduled auction for this product
            var productSpecs = new BaseSpecifications<Product>(p => p.Id == dto.ProductId && p.HasActiveAcution);
            var productWithActiveAuc = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(productSpecs);
            if (productWithActiveAuc is not null)
                return ApiResponse<Auction>.Fail("هذا المنتج لديه مزاد نشط بالفعل");

            // 3. Validate dates
            if (dto.StartDate <= DateTime.UtcNow)
                return ApiResponse<Auction>.Fail("تاريخ البدء يجب أن يكون في المستقبل");
            if (dto.EndDate <= dto.StartDate.AddHours(1))
                return ApiResponse<Auction>.Fail("يجب أن يستمر المزاد لمدة ساعة واحدة على الأقل");

            // 4. Validate prices
            if (dto.ReservePrice.HasValue && dto.ReservePrice < dto.StartingPrice)
                return ApiResponse<Auction>.Fail("سعر الاحتياطي يجب أن يكون أكبر من أو يساوي سعر البدء");

            // 5. Create auction
            var auction = _mapper.Map<Auction>(dto);
            auction.FarmerId = farmerId;
            // 6. Reserve the product
            //product.Status = ProductStatus.Reserved;
            product.HasActiveAcution = true;

            await _unitOfWork.Repository<Auction>().AddAsync(auction);
            if(await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<Auction>.Fail("فشل في إنشاء المزاد حاول لاحقاً");

            return ApiResponse<Auction>.Ok(auction,"تم إنشاء المزاد بنجاح");
        }

        public async Task<ApiResponse<string>> CancelAuctionAsync(string farmerId, int auctionId)
        {
            var auctionSpecs = new AuctionSpecifications(a => a.Id == auctionId,false,true);
            var auction = await _unitOfWork.Repository<Auction>().GetByIdWithSpecAsync(auctionSpecs);

            if (auction is null)
                return ApiResponse<string>.Fail("هذا المزاد غير موجود");
            if(auction.FarmerId !=  farmerId)
                return ApiResponse<string>.Fail("هذا المزاد ليس ملكك");
            if(auction.Status != AuctionStatus.Scheduled)
                return ApiResponse<string>.Fail("هذا المزاد لا يمكنك إلغائه");

            auction.Status = AuctionStatus.Cancelled;
            auction.Product!.HasActiveAcution = false;
            await _unitOfWork.CompleteAsync();
            return ApiResponse<string>.Ok(auctionId.ToString(), "تم إلغاء المزاد بنجاح");
        }

        public async Task<ApiResponse<BidResponseDto>> PlaceBidAsync(string bidderId, int auctionId, PlaceBidDto dto)
        {
            // 1. Get auction
            var auctionSpecs = new AuctionSpecifications(a => a.Id == auctionId,true);
            var auction = await _unitOfWork.Repository<Auction>().GetByIdWithSpecAsync(auctionSpecs);
            if (auction is null)
                return ApiResponse<BidResponseDto>.Fail("هذا المزاد غير متوفر");

            // 2. Validate auction is active
            if (auction.Status != AuctionStatus.Active)
                return ApiResponse<BidResponseDto>.Fail("المزاد غير متاح الان");

            // 3. Farmer cannot bid on own auction
            if (auction.FarmerId == bidderId)
                return ApiResponse<BidResponseDto>.Fail("لا يمكنك المزايدة على مزادك الخاص");

            // 4. Check if bidder's bid is already winning
            var existingWinningBid = auction.Bids.FirstOrDefault(b => b.IsWinning);
            if (existingWinningBid?.BidderId == bidderId)
                return ApiResponse<BidResponseDto>.Fail("لا تزال مزايدتك الأخيرة هي الأعلي متخلكش عبيط");

            // 5. Validate bid amount
            var minBidIncrement = auction.CurrentPrice * 0.05m; // 5% increment
            var minimumBid = auction.CurrentPrice + minBidIncrement;

            if (dto.Amount < minimumBid)
                return ApiResponse<BidResponseDto>.Fail($"الحد الأدنى للمزايدة هو {minimumBid:F2}. " +$"يجب أن تكون على الأقل 5% أعلى من السعر الحالي.");

            // 6. Remove winning flag from previous bid
            if (existingWinningBid is not null)
                existingWinningBid.IsWinning = false;

            // 7. Create new bid
            var bid = new Bid
            {
                AuctionId = auctionId,
                BidderId = bidderId,
                Amount = dto.Amount,
                IsWinning = true,
                BidTime = DateTime.UtcNow
            };

            // 8. Update auction current price
            auction.CurrentPrice = dto.Amount;

            auction.Bids.Add(bid);
            auction.WinnerId = bidderId;
            if (await _unitOfWork.CompleteAsync() <= 0)
                return ApiResponse<BidResponseDto>.Fail("فشل إضافة المزايدة حاول لاحقاً");
            var bidDto = _mapper.Map<BidResponseDto>(bid);
            //bidDto.BidderName = (await _unitOfWork.Repository<ApplicationUser>().GetByIdAsync(bid.BidderId))?.Name;
            return ApiResponse<BidResponseDto>.Ok(bidDto,"تمت إضافة المزايدة بنجاح");
        }

        public async Task ProcessAuctionEndsAsync()
        {
            var auctionSpecs = new AuctionSpecifications(a => a.EndDate <= DateTime.UtcNow && a.Status == AuctionStatus.Active); 
            var endedAuctions = await _unitOfWork.Repository<Auction>().GetAllWithSpecAsync(auctionSpecs);

            foreach (var auction in endedAuctions)
            {
                var winningBid = auction.Bids.FirstOrDefault(b => b.IsWinning);

                // No bids or reserve price not met
                bool reserveMet = auction.ReservePrice is null || auction.CurrentPrice >= auction.ReservePrice;

                if (winningBid is null || !reserveMet)
                {
                    auction.Status = AuctionStatus.Failed;
                    //auction.Product!.Status = ProductStatus.Active; // release product
                    auction.Product!.HasActiveAcution = false; 
                }
                else
                {// Auction succeeded

                    // Auto-create order and payment intend for the winner
                    try
                    {
                        await CreateOrderForWinnerAsync(auction, winningBid);
                    }
                    catch (Exception ) { return; }

                    // Auction succeeded
                    auction.Status = AuctionStatus.Ended;
                    auction.Product!.HasActiveAcution = false; // release product
                    auction.WinnerId = winningBid.BidderId;
                    auction.EndDate = DateTime.UtcNow; // I can leave it as it is
                    // Notify winner
                    await _emailService.SendAsync(auction.Winner!.Email!, "لقد فزت بالمزاد بنجاح",
                      $"لقد فزت بالمزاد علي منتج {auction.Product!.Name} " +$" مقابل {auction.CurrentPrice} EGP"
                    );

                    // Notify farmer
                    await _emailService.SendAsync(auction.Farmer!.Email!,"إنتهاء المزاد بنجاح",
                        $"المزاد علي المنتج " + $"{auction.Product.Name} تم بيعه  " +$" مقابل {auction.CurrentPrice} EGP"
                    );
                }
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task ProcessAuctionStartsAsync()
        {

            var auctionSpecs = new BaseSpecifications<Auction>(a => a.StartDate <= DateTime.UtcNow && a.Status == AuctionStatus.Scheduled);
            var startedAuctions = await _unitOfWork.Repository<Auction>().GetAllWithSpecAsync(auctionSpecs);

            foreach (var auction in startedAuctions)
            {
                auction.StartDate = DateTime.UtcNow; // I can leave it as it is
                auction.Status = AuctionStatus.Active; // now it can receive bids 
                _unitOfWork.Repository<Auction>().Update(auction);
            }
           
             await _unitOfWork.CompleteAsync();
        }

        private async Task CreateOrderForWinnerAsync(Auction auction, Bid winningBid)
        {
            var addSpecs = new BaseSpecifications<Address>(a => a.UserId == winningBid.BidderId && a.IsDefault);
            var winnerAddress = await _unitOfWork.Repository<Address>().GetByIdWithSpecAsync(addSpecs);
            
            // Get the items 
            var orderItems = new List<OrderItem>() {  // only one item
                new OrderItem
            {
                //ProductId        = auction.ProductId,
                ItemOrdered = new ProductItemOrdered()
                {
                    ProductId = auction.ProductId,
                    ProductName = auction.Product!.Name,
                    PictureUrl = "",
                    Unit = auction.Product!.Unit
                },
                Quantity = auction.Product!.Quantity,
                UnitPriceAtOrder = winningBid.Amount/auction.Product!.Quantity,
                Subtotal = winningBid.Amount
            }};


            // Get DeliveryMethod from the delivery method repository
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(4);
            
            var orderAddress = new OrderAddress()
            {
                Street = winnerAddress.Street,
                City = winnerAddress.City,
                Governorate = winnerAddress.Governorate,
                Country = winnerAddress.Country,
                Name = ""
            };

            var orderRepo = _unitOfWork.Repository<Order>();

            var paymentInfo = await _paymentService.CreatePaymentIntentAutomaticAsync(winningBid.Amount);

            //Create an order
            var order = new Order(auction.Winner!.Email!, winningBid.BidderId, winningBid.Amount,"تم إنشاء هذا الطلب تلقائي نتيجة الفوز في المزاد", orderAddress, orderItems, deliveryMethod, paymentInfo.Key);
            order.Status = OrderStatus.Confirmed;
            order.FarmerId = auction.FarmerId;
            await orderRepo.AddAsync(order);

            await _unitOfWork.CompleteAsync();

            var addressSpecForFarmer = new BaseSpecifications<Address>(ad => ad.UserId == auction.FarmerId && ad.IsDefault);
            var defaultFarmerAddress = await _unitOfWork.Repository<Address>().GetByIdWithSpecAsync(addressSpecForFarmer);

            //Create Logistics record automatically
            var logistics = new Logistics
            {
                OrderId = order.Id,
                PickupAddressId = defaultFarmerAddress!.Id,
                DeliveryAddressId = winnerAddress.Id,
                Status = LogisticsStatus.Scheduled,
                EstimatedDelivery = DateTime.Now.AddDays(10)
            };
            await _unitOfWork.Repository<Logistics>().AddAsync(logistics);

            //Payment Record
            var payment = new Payment()
            {
                OrderId = order.Id,
                PayerId = auction.WinnerId!,
                Amount = order.GetTotal(),
                Method = PaymentMethod.Card,
                Status = PaymentStatus.Unpaid,
                PaymentIntentId = paymentInfo.Key
            };
            await _unitOfWork.Repository<Payment>().AddAsync(payment);

            //Save to the database
            //await _unitOfWork.CompleteAsync();
            auction.Product.Status = ProductStatus.SoldOut;
            auction.Product.Quantity = 0;
        }
        //farmer
        public async Task<ApiResponse<IReadOnlyList<AuctionResponseWithNoBidsDto>>> GetMyAuctionsAsync(string farmerId)
        {
            var auctionSpecs = new AuctionSpecifications(a => a.FarmerId == farmerId);//bids, product, farmer,winner if exist
            var auctions = await _unitOfWork.Repository<Auction>().GetAllWithSpecAsync(auctionSpecs);

            if (!auctions.Any())
                return ApiResponse<IReadOnlyList<AuctionResponseWithNoBidsDto>>.Fail("لا يوجد مزادات لعرضها");

            var auctionDtos = new List<AuctionResponseWithNoBidsDto>();//auctions.Select(a => _mapper.Map<AuctionResponseWithNoBidsDto>(a)).ToList();
            foreach (var auction in auctions)
            {
                var auctionDto = _mapper.Map<AuctionResponseWithNoBidsDto>(auction);
                //auctionDto.Bids = auction.Bids.Select(b => _mapper.Map<BidResponseDto>(b)).ToList();
                var productImageSpecs = new BaseSpecifications<ProductImage>(pi => pi.ProductId == auction.ProductId && pi.IsMain);
                var mainProductImage = await _unitOfWork.Repository<ProductImage>().GetByIdWithSpecAsync(productImageSpecs);
                auctionDto.MainImageUrl = $"{_baseUrl}{mainProductImage?.ImageUrl}";
                auctionDto.FarmerImage = $"{_baseUrl}{auctionDto.FarmerImage}";
                auctionDto.WinnerImage = $"{_baseUrl}{auctionDto.WinnerImage}";
                auctionDtos.Add(auctionDto);
            }
            return ApiResponse<IReadOnlyList<AuctionResponseWithNoBidsDto>>.Ok(auctionDtos,"تم الحصول علي مزاداتك بنجاح");
        }

        public async Task<ApiResponse<AuctionResponseDto>> GetByIdAsync(int auctionId)
        {
           var auctionSpecs = new AuctionSpecifications(a => a.Id  == auctionId);
           var auction = await _unitOfWork.Repository<Auction>().GetByIdWithSpecAsync(auctionSpecs);// with bids(مش محتاجها هنا لاني بضطر اجيببها تاني بس مع bidder), product, farmer,winner if exist

            if (auction is null)
                return ApiResponse<AuctionResponseDto>.Fail("هذا المزاد غير موجود");

            var auctionDto = _mapper.Map<AuctionResponseDto>(auction);
            //auctionDto.Bids = auction.Bids.Select(b => _mapper.Map<BidResponseDto>(b)).ToList();
            var bidSpecs = new BidSpecifications(b => b.AuctionId == auctionId);
            var bidsWithBiders = await _unitOfWork.Repository<Bid>().GetAllWithSpecAsync(bidSpecs);

            foreach (var b in bidsWithBiders)
            {
                var bidDto = _mapper.Map<BidResponseDto>(b);
                bidDto.BidderName = b.Bidder.Name;
                bidDto.BidderImage = $"{_baseUrl}{b.Bidder.ProfileImageUrl}" ??" ";
                auctionDto.Bids.Add(bidDto);
            }
                
            var productImageSpecs = new BaseSpecifications<ProductImage>(pi => pi.ProductId == auction.ProductId && pi.IsMain);
            var mainProductImage = await _unitOfWork.Repository<ProductImage>().GetByIdWithSpecAsync(productImageSpecs);
            auctionDto.MainImageUrl = $"{_baseUrl}{mainProductImage?.ImageUrl}" ?? "";
            auctionDto.FarmerImage = $"{_baseUrl}{auction.Farmer?.ProfileImageUrl}" ?? "";
            auctionDto.WinnerImage = $"{_baseUrl}{auction.Winner?.ProfileImageUrl}" ?? "";

            return ApiResponse<AuctionResponseDto>.Ok(auctionDto, "تم الحصول علي تفاصيل المزاد بنجاح");
        }

        public async Task<ApiResponse<AuctionResponseDto>> GetByProductIdAsync(int productId)
        {
            var auctionSpecs = new AuctionSpecifications(a => a.ProductId == productId);
            var auction = await _unitOfWork.Repository<Auction>().GetByIdWithSpecAsync(auctionSpecs);

            if (auction is null)
                return ApiResponse<AuctionResponseDto>.Fail("هذا المزاد غير موجود");

            var auctionDto = _mapper.Map<AuctionResponseDto>(auction);
            auctionDto.Bids = auction.Bids.Select(b => _mapper.Map<BidResponseDto>(b)).ToList();
            var productImageSpecs = new BaseSpecifications<ProductImage>(pi => pi.ProductId == auction.ProductId && pi.IsMain);
            var mainProductImage = await _unitOfWork.Repository<ProductImage>().GetByIdWithSpecAsync(productImageSpecs);
            auctionDto.MainImageUrl = mainProductImage?.ImageUrl ?? "";

            return ApiResponse<AuctionResponseDto>.Ok(auctionDto, "تم الحصول علي تفاصيل المزاد بنجاح");
        }

        public async Task<ApiResponse<Pagination<AuctionResponseWithNoBidsDto>>> GetAllAsync(AuctionSpecParams filter) // with bids(مش محتاجها هنا), product, farmer,winner if exist
        {
            var auctionSpecs = new AuctionSpecifications(filter);
            var auctions = await _unitOfWork.Repository<Auction>().GetAllWithSpecAsync(auctionSpecs);
            if (!auctions.Any())
                return ApiResponse<Pagination<AuctionResponseWithNoBidsDto>>.Fail("لا يوجد مزادات تم إنشائها لعرضها");

            var countSpecs = new BaseSpecifications<Auction>(auctionSpecs.Criteria!);
            var count = await _unitOfWork.Repository<Auction>().GetCountAsync(countSpecs);

            var auctionDtos = new List<AuctionResponseWithNoBidsDto>(); 
            foreach ( var auction in auctions)
            {
                var auctionDto = _mapper.Map<AuctionResponseWithNoBidsDto>(auction);
                //auctionDto.Bids = auction.Bids.Select(b => _mapper.Map<BidResponseDto>(b)).ToList();
                var productImageSpecs = new BaseSpecifications<ProductImage>(pi => pi.ProductId == auction.ProductId && pi.IsMain);
                var mainProductImage = await _unitOfWork.Repository<ProductImage>().GetByIdWithSpecAsync(productImageSpecs);
                auctionDto.MainImageUrl = $"{_baseUrl}{mainProductImage?.ImageUrl}";
                auctionDto.FarmerImage = $"{_baseUrl}{auctionDto.FarmerImage}";
                auctionDto.WinnerImage = $"{_baseUrl}{auctionDto.WinnerImage}";
                auctionDtos.Add(auctionDto);
            }

            var pagination = new Pagination<AuctionResponseWithNoBidsDto>(filter.PageIndex,filter.pageSize,count,auctionDtos);

            return ApiResponse<Pagination<AuctionResponseWithNoBidsDto>>.Ok(pagination, "تم الحصول علي المزادات بنجاح");
        }

        public async Task<ApiResponse<IReadOnlyList<BidResponseDto>>> GetBidsAsync(int auctionId)
        {
            var bidSpecs = new BaseSpecifications<Bid>(b => b.AuctionId == auctionId);
            var bids = await _unitOfWork.Repository<Bid>().GetAllWithSpecAsync(bidSpecs);
            if (!bids.Any())
                return ApiResponse<IReadOnlyList<BidResponseDto>>.Fail("لا يوجد مزايدات علي هذا المزاد");
            var bidDtos = bids.Select(b => _mapper.Map<BidResponseDto>(b)).ToList();

            return ApiResponse<IReadOnlyList<BidResponseDto>>.Ok(bidDtos, "تم الحصول علي المزايدات بنجاح");
        }

        public async Task<ApiResponse<IReadOnlyList<BidResponseDto>>> GetMyBidsAsync(string bidderId)
        {
            var bidSpecs = new BaseSpecifications<Bid>(b => b.BidderId == bidderId);
            var bids = await _unitOfWork.Repository<Bid>().GetAllWithSpecAsync(bidSpecs);
            if (!bids.Any())
                return ApiResponse<IReadOnlyList<BidResponseDto>>.Fail("لا يوجد مزادات مرتبطة بهذا التاجر");

            var bidDtos = bids.Select(b => _mapper.Map<BidResponseDto>(b)).ToList();
            return ApiResponse<IReadOnlyList<BidResponseDto>>.Ok(bidDtos,"تم الحصول علي كل المزيدات التي قمت بها");
        }

        public async Task<ApiResponse<IReadOnlyList<MyWinningtAuctionsDto>>> GetMyWinningtAuctions(string bidderId) // نخليها اللي فزت فيها
        {
            var bidsSpecs = new BaseSpecifications<Bid>(b => b.BidderId == bidderId && b.IsWinning);
            var bids = await _unitOfWork.Repository<Bid>().GetAllWithSpecAsync(bidsSpecs);
            if (!bids.Any())
                return ApiResponse<IReadOnlyList<MyWinningtAuctionsDto>>.Fail("لا يوجد مزايدات فزت من خلالها بمزادات بعد");

            var distincitAuctionIds = bids.Select(b => b.AuctionId)/*.Distinct()*/;
            var dtos = new List<MyWinningtAuctionsDto>();
            foreach(var id in distincitAuctionIds)
            {
                var auctionSpecs = new AuctionSpecifications(a => a.Id == id,verylighted:true); //product farmer
                var auction = await _unitOfWork.Repository<Auction>().GetByIdWithSpecAsync(auctionSpecs);
                var dto = _mapper.Map<MyWinningtAuctionsDto>(auction);
                var productImageSpecs = new BaseSpecifications<ProductImage>(pi => pi.ProductId == auction!.ProductId && pi.IsMain);
                var mainProductImage = await _unitOfWork.Repository<ProductImage>().GetByIdWithSpecAsync(productImageSpecs);
                dto.MainImageUrl = $"{_baseUrl}{mainProductImage?.ImageUrl??""}";
                dto.FarmerImage = $"{_baseUrl}{dto.FarmerImage}";
                dtos.Add(dto);
            }

            return ApiResponse<IReadOnlyList<MyWinningtAuctionsDto>>.Ok(dtos, "تم الحصول علي المزادات التي فزت بها");
            
             

        }
    }
}
