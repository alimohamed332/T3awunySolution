using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Auction
{
    public class AuctionSummaryDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal? ReservePrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public bool IsEnded => DateTime.UtcNow >= EndDate;
        public double MinutesRemaining =>
            Math.Max(0, (EndDate - DateTime.UtcNow).TotalMinutes);
        
    }
}
