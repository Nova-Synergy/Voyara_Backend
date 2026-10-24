using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Identity;

namespace Voyara.Infrastructure.Persistence.Configurations.Identity
{
    /// <summary>
    /// Configuration for the User entity type.
    /// Roles are mapped as a many-to-many relationship and refresh tokens are owned by the user.
    /// </summary>
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        /// <summary>
        /// Configures the entity type for the User model using the specified builder.
        /// </summary>
        /// <remarks>This method sets up primary keys, property constraints, relationships, and indexes for the
        /// User entity. It is typically called by the Entity Framework infrastructure when building the
        /// model.</remarks>
        /// <param name="builder">The builder used to configure the User entity type and its properties.</param>
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Configure the table name and the primary key for the User entity.
            builder.ToTable("users");
            builder.HasKey(u => u.Id);

            // Configure the FullName property for the User entity.
            builder.Property(u => u.FullName).HasMaxLength(200).IsRequired();

            // Configure the Email property for the User entity.
            builder.Property(u => u.Email).HasMaxLength(254).IsRequired();

            // Configure the PasswordHash property for the User entity.
            builder.Property(u => u.PasswordHash).HasMaxLength(500).IsRequired();

            // Configure the EmailConfirmed and IsActive flags for the User entity.
            builder.Property(u => u.EmailConfirmed).IsRequired();
            builder.Property(u => u.IsActive).IsRequired();

            // Configure the audit properties inherited from AuditableEntity.
            builder.Property(u => u.CreatedBy).HasMaxLength(100);
            builder.Property(u => u.UpdatedBy).HasMaxLength(100);

            // E-mail is stored normalized (trimmed, lower-case) by the domain, so a plain unique index is enough.
            builder.HasIndex(u => u.Email).IsUnique();

            // Soft delete: deleted users are hidden from every query.
            builder.HasQueryFilter(u => !u.IsDeleted);

            // Roles: many-to-many through user_roles; Role has no back navigation.
            builder.HasMany(u => u.Roles)
                .WithMany()
                .UsingEntity(join => join.ToTable("user_roles"));
            builder.Navigation(u => u.Roles).UsePropertyAccessMode(PropertyAccessMode.Field);

            // Composition: refresh tokens live and die with their user.
            builder.HasMany(u => u.RefreshTokens)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(u => u.RefreshTokens).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
