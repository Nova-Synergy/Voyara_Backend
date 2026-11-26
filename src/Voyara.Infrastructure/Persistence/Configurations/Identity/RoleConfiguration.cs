using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Application.Abstractions.Authorization;
using Voyara.Domain.Entities.Identity;

namespace Voyara.Infrastructure.Persistence.Configurations.Identity
{

    /// <summary>
    /// Represents the configuration for the Role entity type.
    /// The many-to-many relationship with User is configured in UserConfiguration.
    /// </summary>
    public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        /// <summary>
        /// Configures the Role entity type.
        /// </summary>
        /// <param name="builder">The builder used to configure the Role entity type.</param>
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Configures the Role entity.
            builder.HasKey(x => x.Id);

            // Configures the Name property.
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Guarantees that role names are unique.
            builder.HasIndex(x => x.Name)
                .IsUnique();

            // Seeds the application roles (ids are fixed so migrations stay deterministic).
            builder.HasData(
                new { Id = 1, Name = Roles.Tourist },
                new { Id = 2, Name = Roles.TourGuide },
                new { Id = 3, Name = Roles.Admin });
        }
    }
}
