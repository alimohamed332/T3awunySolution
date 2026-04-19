using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;

namespace T3awuny.Infrastructure.Configurations
{
    internal class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.JoinDate)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(u => u.FarmerProfile)
                .WithOne(fp => fp.User)
                .HasForeignKey<FarmerProfile>(f => f.FarmerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(u => u.TraderProfile)
                .WithOne(tp => tp.User)
                .HasForeignKey<TraderProfile>(t => t.TraderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Addresses)
                .WithOne()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsMany(u => u.RefreshTokens)
                .WithOwner()
                .HasForeignKey(rt => rt.UserId);

        }
    }
}
