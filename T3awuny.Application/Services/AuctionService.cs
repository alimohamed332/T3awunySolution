using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Common;
using T3awuny.Application.Contracts;
using T3awuny.Core;
using T3awuny.Core.Entities.AuctionModule;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;

namespace T3awuny.Application.Services
{
    public class AuctionService : IAuctionService
    {
        //private readonly IUnitOfWork _unitOfWork;
        //private readonly IHubContext<AuctionHub> _hubContext;  // SignalR
        //private readonly INotificationService _notificationService;

        //// ─── Create ───────────────────────────────────────────
        //public async Task<ApiResponse<AuctionResponseDto>> CreateAuctionAsync(
        //    string farmerId, CreateAuctionDto dto)
        //{
        //    // 1. Validate product ownership
        //    var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId);
        //    if (product is null || product.FarmerId != farmerId)
        //        return ApiResponse<AuctionResponseDto>.Fail("Product not found");

        //    if (product.Status != ProductStatus.Active)
        //        return ApiResponse<AuctionResponseDto>.Fail(
        //            "Product must be Active to create an auction");

        //    // 2. Check no existing active/scheduled auction for this product
        //    var existingAuction = await _unitOfWork.Auctions
        //        .GetActiveByProductIdAsync(dto.ProductId);
        //    if (existingAuction is not null)
        //        return ApiResponse<AuctionResponseDto>.Fail(
        //            "This product already has an active auction");

        //    // 3. Validate dates
        //    if (dto.StartDate <= DateTime.UtcNow)
        //        return ApiResponse<AuctionResponseDto>.Fail(
        //            "Start date must be in the future");

        //    if (dto.EndDate <= dto.StartDate.AddHours(1))
        //        return ApiResponse<AuctionResponseDto>.Fail(
        //            "Auction must run for at least 1 hour");

        //    // 4. Validate prices
        //    if (dto.ReservePrice.HasValue && dto.ReservePrice < dto.StartingPrice)
        //        return ApiResponse<AuctionResponseDto>.Fail(
        //            "Reserve price must be >= starting price");

        //    // 5. Create auction
        //    var auction = new Auction
        //    {
        //        ProductId = dto.ProductId,
        //        FarmerId = farmerId,
        //        StartDate = dto.StartDate,
        //        EndDate = dto.EndDate,
        //        StartingPrice = dto.StartingPrice,
        //        ReservePrice = dto.ReservePrice,
        //        CurrentPrice = dto.StartingPrice,
        //        Status = AuctionStatus.Scheduled,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    // 6. Reserve the product
        //    product.Status = ProductStatus.Reserved;

        //    await _unitOfWork.Auctions.AddAsync(auction);
        //    await _unitOfWork.SaveChangesAsync();

        //    return ApiResponse<AuctionResponseDto>.Ok(MapToDto(auction));
        //}

        //// ─── Place Bid ────────────────────────────────────────
        //public async Task<ApiResponse<BidResponseDto>> PlaceBidAsync(
        //    string bidderId, int auctionId, PlaceBidDto dto)
        //{
        //    // 1. Get auction
        //    var auction = await _unitOfWork.Auctions
        //                                   .GetWithBidsAsync(auctionId);
        //    if (auction is null)
        //        return ApiResponse<BidResponseDto>.Fail("Auction not found");

        //    // 2. Validate auction is active
        //    if (auction.Status != AuctionStatus.Active)
        //        return ApiResponse<BidResponseDto>.Fail("Auction is not active");

        //    // 3. Farmer cannot bid on own auction
        //    if (auction.FarmerId == bidderId)
        //        return ApiResponse<BidResponseDto>.Fail(
        //            "You cannot bid on your own auction");

        //    // 4. Check if bidder's bid is already winning
        //    var existingWinningBid = auction.Bids
        //        .FirstOrDefault(b => b.IsWinning);
        //    if (existingWinningBid?.BidderId == bidderId)
        //        return ApiResponse<BidResponseDto>.Fail(
        //            "Your bid is already the highest");

        //    // 5. Validate bid amount
        //    var minBidIncrement = auction.CurrentPrice * 0.05m; // 5% increment
        //    var minimumBid = auction.CurrentPrice + minBidIncrement;

        //    if (dto.Amount < minimumBid)
        //        return ApiResponse<BidResponseDto>.Fail(
        //            $"Minimum bid is {minimumBid:F2}. " +
        //            $"Must be at least 5% above current price.");

        //    // 6. Remove winning flag from previous bid
        //    if (existingWinningBid is not null)
        //        existingWinningBid.IsWinning = false;

        //    // 7. Create new bid
        //    var bid = new Bid
        //    {
        //        AuctionId = auctionId,
        //        BidderId = bidderId,
        //        Amount = dto.Amount,
        //        IsWinning = true,
        //        BidTime = DateTime.UtcNow
        //    };

        //    // 8. Update auction current price
        //    auction.CurrentPrice = dto.Amount;

        //    await _unitOfWork.Bids.AddAsync(bid);
        //    await _unitOfWork.SaveChangesAsync();

        //    // 9. Broadcast to all watchers via SignalR
        //    await _hubContext.Clients
        //                     .Group($"auction_{auctionId}")
        //                     .SendAsync("BidPlaced", new
        //                     {
        //                         AuctionId = auctionId,
        //                         BidderName = bid.Bidder?.FullName,
        //                         Amount = dto.Amount,
        //                         CurrentPrice = auction.CurrentPrice,
        //                         BidTime = bid.BidTime
        //                     });

        //    return ApiResponse<BidResponseDto>.Ok(MapToBidDto(bid));
        //}

        //// ─── Process Ended Auctions ───────────────────────────
        //public async Task ProcessAuctionEndsAsync()
        //{
        //    var endedAuctions = await _unitOfWork.Auctions
        //        .GetExpiredActiveAuctionsAsync(DateTime.UtcNow);

        //    foreach (var auction in endedAuctions)
        //    {
        //        var winningBid = auction.Bids.FirstOrDefault(b => b.IsWinning);

        //        // No bids or reserve price not met
        //        bool reserveMet = auction.ReservePrice is null ||
        //                          auction.CurrentPrice >= auction.ReservePrice;

        //        if (winningBid is null || !reserveMet)
        //        {
        //            auction.Status = AuctionStatus.Failed;
        //            auction.Product.Status = ProductStatus.Active; // release product
        //        }
        //        else
        //        {
        //            // Auction succeeded
        //            auction.Status = AuctionStatus.Ended;
        //            auction.WinnerId = winningBid.BidderId;

        //            // Auto-create order for the winner
        //            await CreateOrderForWinnerAsync(auction, winningBid);

        //            // Notify winner
        //            await _notificationService.SendAsync(new Notification
        //            {
        //                UserId = winningBid.BidderId,
        //                Title = "🎉 You won the auction!",
        //                Message = $"You won {auction.Product.Name} " +
        //                                    $"for {auction.CurrentPrice} EGP",
        //                Type = NotificationType.AuctionAlert,
        //                RelatedEntityId = auction.Id.ToString(),
        //                RelatedEntityType = "Auction"
        //            });

        //            // Notify farmer
        //            await _notificationService.SendAsync(new Notification
        //            {
        //                UserId = auction.FarmerId,
        //                Title = "Auction ended successfully",
        //                Message = $"{auction.Product.Name} sold " +
        //                                    $"for {auction.CurrentPrice} EGP",
        //                Type = NotificationType.AuctionAlert,
        //                RelatedEntityId = auction.Id.ToString(),
        //                RelatedEntityType = "Auction"
        //            });
        //        }
        //    }

        //    await _unitOfWork.SaveChangesAsync();
        //}

        //private async Task CreateOrderForWinnerAsync(Auction auction, Bid winningBid)
        //{
        //    var winnerAddress = await _unitOfWork.Addresses
        //                                         .GetDefaultByUserIdAsync(winningBid.BidderId);
        //    var order = new Order
        //    {
        //        BuyerId = winningBid.BidderId,
        //        DeliveryAddressId = winnerAddress!.Id,
        //        TotalAmount = winningBid.Amount,
        //        Status = OrderStatus.Confirmed,  // auto confirmed
        //        PaymentStatus = PaymentStatus.Unpaid,
        //        OrderDate = DateTime.UtcNow,
        //        CreatedAt = DateTime.UtcNow,
        //        Notes = $"Auto-created from Auction #{auction.Id}",
        //        Items = new List<OrderItem>
        //    {
        //        new OrderItem
        //        {
        //            ProductId        = auction.ProductId,
        //            Quantity         = auction.Product.Quantity,
        //            UnitPriceAtOrder = winningBid.Amount,
        //            Subtotal         = winningBid.Amount
        //        }
        //    }
        //    };

        //    await _unitOfWork.Orders.AddAsync(order);
        //    auction.Product.Status = ProductStatus.SoldOut;
        //}
    }
}
