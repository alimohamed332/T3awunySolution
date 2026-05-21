using System;
using System.Collections.Generic;

namespace T3awuny.Application.DTOs.Auction
{
    public class CreateAuctionDto
    {
        public int ProductId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal? ReservePrice { get; set; }
    }

}
