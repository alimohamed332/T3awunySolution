using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.OrderAggregate;

namespace T3awuny.Infrastructure.Configurations.Order
{
    internal class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(orderItem => orderItem.ItemOrdered, itemOrdered => itemOrdered.WithOwner());
            builder.Property(orderItem => orderItem.UnitPriceAtOrder)
                .HasColumnType("decimal(10,2)");

            builder.Property(orderItem => orderItem.Quantity)
                .HasColumnType("decimal(10,2)");

            builder.Property(orderItem => orderItem.Subtotal)
                .HasColumnType("decimal(10,2)");
        }
    }
}
