using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.OrderAggregate;

namespace T3awuny.Infrastructure.Configurations.OrderConfig
{
    internal class DeliveryMethodConfigurations : IEntityTypeConfiguration<DeliveryMethod>
    {
        public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
        {
            builder.Property(d => d.Cost).HasColumnType("decimal(18,2)");
            builder.Property(d => d.ShortName).HasMaxLength(30);
            builder.Property(d => d.DeliveryTime).HasMaxLength(60);
            builder.Property(d => d.Description).HasMaxLength(100);
        }
    }
}
