using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Identity;

namespace Voyara.Infrastructure.Persistence.Configurations.Identity
{
    /// <summary>
    /// Configuration for the RefreshToken entity type.
    /// The relationship with User is configured in UserConfiguration.
    /// </summary>
    public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        /// <summary>
        /// Configures the entity type for the RefreshToken model using the specified builder.
        /// </summary>
        /// <remarks>This method sets up primary keys, property constraints, relationships, and indexes for the
        /// RefreshToken entity. It is typically called by the Entity Framework infrastructure when building the
        /// model.</remarks>
        /// <param name="builder">The builder used to configure the RefreshToken entity type and its properties.</param>
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            // Configure the table name and the primary key for the RefreshToken entity.
            builder.ToTable("refresh_tokens");
            builder.HasKey(t => t.Id);

            // Configure the TokenHash property for the RefreshToken entity.
            builder.Property(t => t.TokenHash).HasMaxLength(256).IsRequired();

            // Configure the ExpiresAt and RevokedAt properties for the RefreshToken entity.
            builder.Property(t => t.ExpiresAt).IsRequired();
            builder.Property(t => t.RevokedAt);

            // Token lookup on refresh.
            builder.HasIndex(t => t.TokenHash).IsUnique();

            // Loading all the tokens of a user.
            builder.HasIndex(t => t.UserId);
        }
    }
}
