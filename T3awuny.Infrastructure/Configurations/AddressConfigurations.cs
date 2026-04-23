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
    internal class AddressConfigurations : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.Property(a => a.Label)
                .HasConversion<string>()
                .HasDefaultValue(AddressLabel.Home)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(a => a.Street)
                .HasMaxLength(30);

            builder.Property(a => a.City)
                .HasMaxLength(30);

            builder.Property(a => a.Governorate)
                .HasMaxLength(30).HasDefaultValue("الفيوم");

            builder.Property(a => a.Country)
                .HasMaxLength(30).HasDefaultValue("مصر");
        }
    }
}
