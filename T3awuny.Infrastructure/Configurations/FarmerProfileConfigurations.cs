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
    internal class FarmerProfileConfigurations : IEntityTypeConfiguration<FarmerProfile>
    {
        public void Configure(EntityTypeBuilder<FarmerProfile> builder)
        {
            builder.HasKey(f => f.FarmerId);

            builder.Property(f => f.FarmName)
                .HasMaxLength(30);

            builder.Property(f => f.Description)
                .HasMaxLength(150);

            builder.HasOne(f => f.Address)
                .WithOne()
                .HasForeignKey<FarmerProfile>(f => f.FarmAddressId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

