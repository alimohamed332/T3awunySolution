using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Core.Entities.AuctionModule
{
    public class Bid
    {
        public int Id { get; set; }
        public int AuctionId { get; set; }
        //public virtual Auction Auction { get; set; }
        public string BidderId { get; set; } = default!;
        //public virtual ApplicationUser Bidder { get; set; }
        public decimal Amount { get; set; }
        public bool IsWinning { get; set; }          // only one bid is winning at a time for the acution
        public DateTime BidTime { get; set; }
    }
}
