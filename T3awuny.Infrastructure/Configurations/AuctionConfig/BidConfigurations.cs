using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.AuctionModule;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Infrastructure.Configurations.AuctionConfig
{
    public class BidConfigurations : IEntityTypeConfiguration<Bid>
    {
        public void Configure(EntityTypeBuilder<Bid> builder)
        {
            builder.Property(b => b.Amount)
                   .HasPrecision(10, 2);

            builder.Property(b => b.BidTime)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(b => b.Bidder)
                   .WithMany()
                   .HasForeignKey(b => b.BidderId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
