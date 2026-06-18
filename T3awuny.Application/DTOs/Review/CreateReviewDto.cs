using System.ComponentModel.DataAnnotations;


namespace T3awuny.Application.DTOs.Review
{
    public class CreateReviewDto
    {
        public int OrderId { get; set; } 
        public string TargetUserId { get; set; } = string.Empty;
        [Range(1,5, ErrorMessage = "التقيم يجب أن يكون بين 1 و 5")]
        public int Rating { get; set; }           // must be 1-5
        public string? Comment { get; set; }
    }
}
