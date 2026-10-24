using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Enums;

namespace Voyara.Infrastructure.Persistence.Configurations.Identity
{
    /// <summary>
    /// Configuration for the TouristProfile entity type.
    /// It is a one-to-one with User and owns the visit history.
    /// </summary>
    public sealed class TouristProfileConfiguration : IEntityTypeConfiguration<TouristProfile>
    {
        /// <summary>
        /// Configures the entity type for the TouristProfile model using the specified builder.
        /// </summary>
        /// <remarks>This method sets up primary keys, property constraints, relationships, and indexes for the
        /// TouristProfile entity. It is typically called by the Entity Framework infrastructure when building the
        /// model.</remarks>
        /// <param name="builder">The builder used to configure the TouristProfile entity type and its properties.</param>
        public void Configure(EntityTypeBuilder<TouristProfile> builder)
        {
            // Configure the table name and the primary key for the TouristProfile entity.
            builder.ToTable("tourist_profiles");
            builder.HasKey(p => p.Id);

            // Configure the NationalityCode property (ISO 3166-1 alpha-2, stored upper-case).
            builder.Property(p => p.NationalityCode).HasMaxLength(2).IsFixedLength().IsRequired();

            // Configure the PreferredLanguage property (e.g. en, ar, pt-br, stored lower-case).
            builder.Property(p => p.PreferredLanguage).HasMaxLength(10).IsRequired();

            // Configure the Budget property; the BudgetLevel enum is stored as text.
            builder.Property(p => p.Budget).HasConversion<string>().HasMaxLength(20).IsRequired();

            // Configure the PhotoUrl property for the TouristProfile entity.
            builder.Property(p => p.PhotoUrl).HasMaxLength(2048);

            // One profile per user; the profile cannot outlive its user.
            builder.HasOne(p => p.User)
                .WithOne(u => u.TouristProfile)
                .HasForeignKey<TouristProfile>(p => p.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(p => p.UserId).IsUnique();

            // Interests: many-to-many through tourist_profile_interests; Interest has no back navigation.
            builder.HasMany(p => p.Interests)
                .WithMany()
                .UsingEntity(join => join.ToTable("tourist_profile_interests"));
            builder.Navigation(p => p.Interests).UsePropertyAccessMode(PropertyAccessMode.Field);

            // Composition: visit history belongs to the profile.
            builder.HasMany(p => p.VisitHistories)
                .WithOne(v => v.TouristProfile)
                .HasForeignKey(v => v.TouristProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation(p => p.VisitHistories).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
