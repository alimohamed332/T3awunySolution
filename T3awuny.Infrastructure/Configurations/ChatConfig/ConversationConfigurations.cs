using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using T3awuny.Core.Entities.ChatModule;

namespace T3awuny.Infrastructure.Configurations.ChatConfig
{
    public class ConversationConfigurations : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.Property(c => c.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            // prevent duplicate conversations between same two users
            builder.HasIndex(c => new { c.User1Id, c.User2Id })
                   .IsUnique();

            builder.HasOne(c => c.User1)
                   .WithMany()
                   .HasForeignKey(c => c.User1Id)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.User2)
                   .WithMany()
                   .HasForeignKey(c => c.User2Id)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Messages)
                   .WithOne(m => m.Conversation)
                   .HasForeignKey(m => m.ConversationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
