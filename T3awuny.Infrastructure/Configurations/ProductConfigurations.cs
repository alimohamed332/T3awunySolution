using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities;
using T3awuny.Core.Entities.Enums;

namespace T3awuny.Infrastructure.Configurations
{
    public class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(p => p.UnitPrice)
                   .HasPrecision(10, 2);          

            builder.Property(p => p.Quantity)
                   .HasPrecision(10, 3);

            builder.Property(p => p.Status)
                   .HasConversion<string>()
                   .HasDefaultValue(ProductStatus.Draft)
                   .HasMaxLength(20);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.Unit)
                .HasMaxLength(10)
                .HasDefaultValue("كيلو جرام");

            builder.Property(p => p.Description)
                .HasMaxLength(200);

            builder.HasOne(p => p.Farmer)
                   .WithMany()
                   .HasForeignKey(p => p.FarmerId)
                   .OnDelete(DeleteBehavior.Cascade);      

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.Products)
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(p => p.ProductId);

            builder.HasQueryFilter(p => p.Status != ProductStatus.Deleted);

            builder.HasIndex(p => p.Name);
        }
    }
}
