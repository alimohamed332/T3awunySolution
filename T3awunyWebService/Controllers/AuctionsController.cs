using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace T3awunyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionsController : ControllerBase
    {
        //create bid 
        // 9. Broadcast to all watchers via SignalR
        //await _hubContext.Clients
        //                 .Group($"auction_{auctionId}")
        //                     .SendAsync("BidPlaced", new
        //                     {
        //    AuctionId = auctionId,
        //                         BidderName = bid.Bidder?.FullName,
        //                         Amount = dto.Amount,
        //                         CurrentPrice = auction.CurrentPrice,
        //                         BidTime = bid.BidTime
        //});
    }
}
