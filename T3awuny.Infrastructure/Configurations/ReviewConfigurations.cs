using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using T3awuny.Core.Entities.ReviewModule;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Infrastructure.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.Property(r => r.Rating)
                   .IsRequired();

            builder.Property(r => r.Comment)
                .HasMaxLength(100);

            builder.Property(r => r.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // buyer can't review same order twice
            builder.HasIndex(r => new { r.ReviewerId, r.OrderId })
                   .IsUnique();

            builder.HasOne(r => r.Reviewer)
                   .WithMany()
                   .HasForeignKey(r => r.ReviewerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.TargetUser)
                   .WithMany()
                   .HasForeignKey(r => r.TargetUserId)
                   .OnDelete(DeleteBehavior.Restrict);
            //عشان الاستاذ علي نصر مكسل يشتغل هبقي اعملها انا بعدين
            //builder.HasOne(r => r.Order)
            //       .WithMany()
            //       .HasForeignKey(r => r.OrderId)
            //       .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
