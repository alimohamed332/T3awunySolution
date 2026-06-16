using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.AuctionModule;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Infrastructure.Configurations.AuctionConfig
{
    public class AuctionConfigurations : IEntityTypeConfiguration<Auction>
    {
        public void Configure(EntityTypeBuilder<Auction> builder)
        {
            builder.Property(a => a.StartingPrice)
                   .HasPrecision(10, 2);

            builder.Property(a => a.ReservePrice)
                   .HasPrecision(10, 2);

            builder.Property(a => a.CurrentPrice)
                   .HasPrecision(10, 2);

            builder.Property(a => a.Status)
                   .HasConversion<string>()
                   .HasDefaultValue(AuctionStatus.Scheduled);

            builder.Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(a => a.Product)
                   .WithMany()
                   .HasForeignKey(a => a.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Farmer)
                   .WithMany()
                   .HasForeignKey(a => a.FarmerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Winner)
                   .WithMany()
                   .HasForeignKey(a => a.WinnerId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.HasMany(a => a.Bids)
                   .WithOne()
                   .HasForeignKey(b => b.AuctionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
