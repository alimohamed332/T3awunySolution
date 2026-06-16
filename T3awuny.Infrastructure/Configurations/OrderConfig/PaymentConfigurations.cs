using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using T3awuny.Core.Entities.Enums;
using T3awuny.Core.Entities.OrderAggregate;

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

            builder.HasOne(p => p.Payer)
                .WithMany().HasForeignKey(p => p.PayerId);
        }
    }
}
