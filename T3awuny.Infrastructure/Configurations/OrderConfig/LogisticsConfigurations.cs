using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Infrastructure.Configurations.OrderConfig
{
    internal class LogisticsConfigurations : IEntityTypeConfiguration<Logistics>
    {
        public void Configure(EntityTypeBuilder<Logistics> builder)
        {
            builder.Property(lo => lo.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(lo => lo.DriverName)
                .HasMaxLength(20);

            builder.Property(lo => lo.Notes)
                .HasMaxLength(40);

            builder.Property(lo => lo.DriverPhone)
                .HasMaxLength(20);

            builder.HasOne<Address>()
                .WithOne()
                .HasForeignKey<Logistics>(lo => lo.PickupAddressId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<Address>()
                .WithOne()
                .HasForeignKey<Logistics>(lo => lo.DeliveryAddressId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
