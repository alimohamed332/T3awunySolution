using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;

namespace T3awuny.Infrastructure.Configurations
{
    internal class TraderProfileConfigurations : IEntityTypeConfiguration<TraderProfile>
    {
        public void Configure(EntityTypeBuilder<TraderProfile> builder)
        {
            builder.HasKey(f => f.TraderId);

            builder.Property(t => t.BusinessName)
                .HasMaxLength(30);

            builder.Property(t => t.BusinessType)
                .HasDefaultValue(BusinessType.Wholesaler);
        }
    }
}