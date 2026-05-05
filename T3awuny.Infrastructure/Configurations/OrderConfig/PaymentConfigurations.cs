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
    internal class PaymentConfigurations : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(p => p.Amount)
                   .HasPrecision(11, 3);

            builder.Property(p => p.Status)
                   .HasConversion<string>()
                   .HasDefaultValue(PaymentStatus.Unpaid);

            builder.Property(p => p.Method)
                  .HasConversion<string>()
                  .HasDefaultValue(PaymentMethod.Card);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne<ApplicationUser>()
                .WithOne().HasForeignKey<Payment>(p => p.PayerId);
        }
    }
}
