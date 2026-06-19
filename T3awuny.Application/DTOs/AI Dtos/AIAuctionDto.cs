using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.AuctionModule;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Application.DTOs.AI_Dtos
{
    public class AIAuctionDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;//
        public string FarmerId { get; set; } = default!;
        public string FarmerName { get; set; } = string.Empty;//
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal? ReservePrice { get; set; }   // minimum acceptable price
        public decimal CurrentPrice { get; set; }     // updated with every bid
        public string? WinnerId { get; set; }
        public string Status { get; set; } = string.Empty;//
    }
}

