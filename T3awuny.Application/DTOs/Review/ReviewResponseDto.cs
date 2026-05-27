
namespace T3awuny.Application.DTOs.Review
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public string ReviewerName { get; set; } = string.Empty;//
        public string? ReviewerImageUrl { get; set; } //
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
