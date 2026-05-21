
namespace T3awuny.Application.DTOs.Auction
{
    public class BidResponseDto
    {
        public int Id { get; set; }
        public string BidderId { get; set; } = string.Empty;
        //public string BidderName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsWinning { get; set; }
        public DateTime BidTime { get; set; }
    }

}
