using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;
using T3awuny.Core.Entities.UserModule;

namespace T3awuny.Infrastructure.Configurations.OrderConfig
{
    internal class OrderConfigurations : IEntityTypeConfiguration<T3awuny.Core.Entities.OrderAggregate.Order>
    {
        public void Configure(EntityTypeBuilder<T3awuny.Core.Entities.OrderAggregate.Order> builder)
        {
            builder.Property(o => o.SubTotal)
                   .HasPrecision(10, 2);

            builder.Property(o => o.Status)
                   .HasConversion<string>()
                   .HasDefaultValue(OrderStatus.Pending);

            builder.Property(o => o.Notes)
                   .HasMaxLength(100);

            builder.Property(o => o.PaymentStatus)
                   .HasConversion<string>()
                   .HasDefaultValue(PaymentStatus.Unpaid);

            builder.Property(o => o.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(o => o.BuyerId)
                   .OnDelete(DeleteBehavior.SetNull);


            builder.OwnsOne(o => o.DliveryAddress, dm => dm.WithOwner());

            builder.HasMany(o => o.Items)
                   .WithOne()
                   .OnDelete(DeleteBehavior.Cascade);

            //builder.HasOne(o => o.Payment)
            //       .WithOne(p => p.Order)
            //       .HasForeignKey<Payment>(p => p.OrderId);

            builder.HasOne(o => o.Logistics)
                   .WithOne()
                   .HasForeignKey<Logistics>(l => l.OrderId);
        }
    }
    
}
