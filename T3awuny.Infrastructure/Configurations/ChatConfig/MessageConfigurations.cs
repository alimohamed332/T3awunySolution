using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Core.Entities.ChatModule;

namespace T3awuny.Infrastructure.Configurations.ChatConfig
{
    public class MessageConfigurations : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.Property(m => m.SentAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(m => m.Content)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.HasOne(m => m.Sender)
                    .WithMany()
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
