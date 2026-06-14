

using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Core.Entities.ReviewModule
{
    public class Review : BaseEntity
    {
        public int Id { get; set; }
        public string ReviewerId { get; set; } = string.Empty;
        public ApplicationUser Reviewer { get; set; } = new ApplicationUser();
        public string TargetUserId { get; set; } = string.Empty;
        public ApplicationUser TargetUser { get; set; } = new ApplicationUser();
        public int? OrderId { get; set; }
        //public Order? Order { get; set; } = new Order();       // linked to real transaction
        public int Rating { get; set; }           // 1 to 5
        public string? Comment { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
