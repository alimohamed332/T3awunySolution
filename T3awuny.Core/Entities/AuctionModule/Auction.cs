using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Core.Entities.AuctionModule
{
    public class Auction
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; } 
        public string FarmerId { get; set; } = default!;
        public ApplicationUser? Farmer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal? ReservePrice { get; set; }   // minimum acceptable price
        public decimal CurrentPrice { get; set; }     // updated with every bid
        public string? WinnerId { get; set; }
        public ApplicationUser? Winner { get; set; }
        public AuctionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<Bid> Bids { get; set; } = new HashSet<Bid>();
    }
}
