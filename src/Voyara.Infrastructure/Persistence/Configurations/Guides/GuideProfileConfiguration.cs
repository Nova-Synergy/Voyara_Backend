using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Guides;

namespace Voyara.Infrastructure.Persistence.Configurations.Guides
{

    /// <summary>
    /// Represents the configuration for the GuideProfile entity type.
    /// </summary>
    public sealed class GuideProfileConfiguration : IEntityTypeConfiguration<GuideProfile>
    {
        /// <summary>
        /// Configures the GuideProfile entity type.
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<GuideProfile> builder)
        {

            // Configures the GuideProfile entity.
            builder.HasKey(x => x.Id);


            // Configures the UserId property.
            builder.Property(x => x.UserId)
                .IsRequired();

            // Configures the Bio property.
            builder.Property(x => x.Bio)
                .IsRequired()
                .HasMaxLength(1000);

            // Configures the PhotoUrl property.
            builder.Property(x => x.PhotoUrl)
                .IsRequired(false)
                .HasMaxLength(2048);

            // Configures the YearsOfExperience property.
            builder.Property(x => x.YearsOfExperience)
                .IsRequired();

            // Configures the AverageRating property.
            builder.Property(x => x.AverageRating)
                .HasPrecision(3, 2)
                .IsRequired();

            // Configures the ReviewCount property.
            builder.Property(x => x.ReviewCount)
                .IsRequired();

            // Configures the Status property.
            builder.Property(x => x.Status)
                .HasConversion<short>()
                .IsRequired();

            // Configures the RejectionReason property.
            builder.Property(x => x.RejectionReason)
                .IsRequired(false)
                .HasMaxLength(500);

            // Configures the CityId property.
            builder.Property(x => x.CityId)
                .IsRequired();

            // Configures the Languages relationship.
            builder.HasMany(x => x.Languages)
                .WithOne(x => x.GuideProfile)
                .HasForeignKey(x => x.GuideProfileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Configures the Expertise relationship.
            builder.HasMany(x => x.Expertise)
                .WithOne(x => x.GuideProfile)
                .HasForeignKey(x => x.GuideProfileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Configures the Availability relationship.
            builder.HasMany(x => x.Availability)
                .WithOne(x => x.GuideProfile)
                .HasForeignKey(x => x.GuideProfileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Configures the TimeOff relationship.
            builder.HasMany(x => x.TimeOff)
                .WithOne(x => x.GuideProfile)
                .HasForeignKey(x => x.GuideProfileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Configures the Pricing relationship.
            builder.HasMany(x => x.Pricing)
                .WithOne(x => x.GuideProfile)
                .HasForeignKey(x => x.GuideProfileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Configures the Reviews relationship.
            builder.HasMany(x => x.Reviews)
                .WithOne(x => x.GuideProfile)
                .HasForeignKey(x => x.GuideProfileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Configures the GuideRequests relationship.
            builder.HasMany(x => x.GuideRequests)
                .WithOne(x => x.GuideProfile)
                .HasForeignKey(x => x.GuideProfileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            /// Configures the User relationship.
            builder.HasOne(x => x.User)
               .WithOne()
               .HasForeignKey<GuideProfile>(x => x.UserId)
               .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Configures the City relationship.
            builder.HasOne(x => x.City)
                .WithMany()
                .HasForeignKey(x => x.CityId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


            // Supports filtering guide profiles by city and status.
            builder.HasIndex(x => new
            {
                x.CityId,
                x.Status
            });
        }
    }
}