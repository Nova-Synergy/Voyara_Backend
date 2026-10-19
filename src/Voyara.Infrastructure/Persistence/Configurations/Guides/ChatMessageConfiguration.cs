using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Guides;

namespace Voyara.Infrastructure.Persistence.Configurations.Guides
{
    /// <summary>
    /// Configuration for the ChatMessage entity type.
    /// the relationship with ChatThread is configured in ChatThreadConfiguration.
    /// </summary>
    public sealed class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        /// <summary>
        /// Configures the entity type for the ChatMessage model using the specified builder.
        /// </summary>
        /// <remarks>This method sets up primary keys, property constraints, and indexes for the
        /// ChatMessage entity. It is typically called by the Entity Framework infrastructure when building the
        /// model.</remarks>
        /// <param name="builder">The builder used to configure the ChatMessage entity type and its properties.</param>
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            // Configure the primary key for the ChatMessage entity.
            builder.HasKey(x => x.Id);

            // Configure the Text property for the ChatMessage entity.
            builder.Property(x => x.Text)
                .IsRequired()
                .HasMaxLength(4000);

            // Configure the SentAt property for the ChatMessage entity.
            builder.Property(x => x.SentAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()"); ;
            // Loading a conversation in order.
            builder.HasIndex(x => new { x.ChatThreadId, x.SentAt });
        }
    }
}
