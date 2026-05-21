using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace T3awunyWebService.Hubs
{
    public class AuctionHub : Hub
    {
        // User joins auction room to watch live bids
        [HubMethodName("joinauction")]
        public async Task JoinAuction(int auctionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId,$"auction_{auctionId}");
        }

        // User leaves the auction room
        [HubMethodName("leaveauction")]
        public async Task LeaveAuction(int auctionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId,$"auction_{auctionId}");
        }
    }
}
