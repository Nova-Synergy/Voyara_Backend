using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;

namespace Voyara.Infrastructure.Persistence.Configurations.Guides
{
    /// <summary>
    /// Represents the configuration for the GuideRequest entity type.
    /// The GuideProfile relationship is configured in GuideProfileConfiguration.
    /// </summary>
    public sealed class GuideRequestConfiguration : IEntityTypeConfiguration<GuideRequest>
    {

        /// <summary>
        /// Configures the GuideRequest entity type.
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<GuideRequest> builder)
        {
            // Configures the primary key for the GuideRequest entity.
            builder.HasKey(x => x.Id);

            // Configures the PreferredLanguage property for the GuideRequest entity.
            builder.Property(x => x.PreferredLanguage)
                .IsRequired()
                .HasMaxLength(10);

            // Configures the SpecialRequirements property for the GuideRequest entity.
            builder.Property(x => x.SpecialRequirements)
                .IsRequired(false)
                .HasMaxLength(1000);

            // Configures the Status property for the GuideRequest entity.
            builder.Property(x => x.Status)
                .HasConversion<short>()
                .IsRequired();

            // Configures the RejectionReason property for the GuideRequest entity.
            builder.Property(x => x.RejectionReason)
                .IsRequired(false)
                .HasMaxLength(500);


            // Relations: one-to-one dependents (WithOne creates the unique index on GuideRequestId)
            builder.HasOne(x => x.GuideSession)
                .WithOne(x => x.GuideRequest)
                .HasForeignKey<GuideSession>(x => x.GuideRequestId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // A request can have several threads (team decision), removed together with the request.
            builder.HasMany(x => x.ChatThreads)
                .WithOne(x => x.GuideRequest)
                .HasForeignKey(x => x.GuideRequestId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // History: a review is never deleted with the request.
            builder.HasOne(x => x.GuideReview)
                .WithOne(x => x.GuideRequest)
                .HasForeignKey<GuideReview>(x => x.GuideRequestId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            // Prevents double-booking an accepted guide slot (ERD-04).
            // Overlapping time ranges are still checked in the transactional availability check (SEQ-09).
            builder.HasIndex(x => new { x.GuideProfileId, x.Date, x.StartTime })
                .IsUnique();


            // Guide inbox and tourist "my requests" lists.
            builder.HasIndex(x => new { x.GuideProfileId, x.Status });
            builder.HasIndex(x => new { x.UserId, x.Status });

            // Background job that expires unanswered requests.
            builder.HasIndex(x => new { x.Status, x.ExpiresAt });
        }
    }
}