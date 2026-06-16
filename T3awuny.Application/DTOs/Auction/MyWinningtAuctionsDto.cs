using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.DTOs.Auction
{
    public class MyWinningtAuctionsDto
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal StartingPrice { get; set; }
        //public decimal? ReservePrice { get; set; }
        public decimal CurrentPrice { get; set; }
        //public int TotalBids { get; set; }
        //public decimal MyHighestBid { get; set; }
        public decimal HighestBid { get; set; }
        public bool IsEnded => DateTime.UtcNow >= EndDate;
        public double MinutesRemaining =>
            Math.Max(0, (EndDate - DateTime.UtcNow).TotalMinutes);

        // Product info
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductUnit { get; set; } = string.Empty;
        public decimal ProductQuantity { get; set; }
        public string? MainImageUrl { get; set; }

        // Farmer info
        public string FarmerId { get; set; } = string.Empty;
        public string FarmerName { get; set; } = string.Empty;
        public string FarmerImage { get; set; } = string.Empty;

        //// Winner (after auction ends)
        //public string? WinnerId { get; set; }
        //public string? WinnerName { get; set; }
        //public string? WinnerImage { get; set; }

    }
}
