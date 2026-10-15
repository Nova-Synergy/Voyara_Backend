using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Guides;


namespace Voyara.Infrastructure.Persistence.Configurations.Guides
{
    /// <summary>
    /// Represents the configuration for the ChatThread entity type.
    /// The GuideRequest relationship is configured in GuideRequestConfiguration.
    /// </summary>
    public sealed class ChatThreadConfiguration : IEntityTypeConfiguration<ChatThread>
    {
        /// <summary>
        /// Configures the entity type for the ChatThread model using the provided builder.
        /// </summary>
        /// <param name="builder">The builder used to configure the ChatThread entity type and its properties.</param>
        public void Configure(EntityTypeBuilder<ChatThread> builder)
        {
            // Configure the primary key for the ChatThread entity.
            builder.HasKey(x => x.Id);

            // Configure the relationship between ChatThread and ChatMessage entities.
            builder.HasMany(x => x.Messages)
                .WithOne(x => x.ChatThread)
                .HasForeignKey(x => x.ChatThreadId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}