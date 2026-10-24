using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Identity;

namespace Voyara.Infrastructure.Persistence.Configurations.Identity
{
    /// <summary>
    /// Configuration for the VisitHistory entity type.
    /// The relationship with TouristProfile is configured in TouristProfileConfiguration.
    /// </summary>
    public sealed class VisitHistoryConfiguration : IEntityTypeConfiguration<VisitHistory>
    {
        /// <summary>
        /// Configures the entity type for the VisitHistory model using the specified builder.
        /// </summary>
        /// <remarks>This method sets up primary keys, property constraints, relationships, and indexes for the
        /// VisitHistory entity. It is typically called by the Entity Framework infrastructure when building the
        /// model.</remarks>
        /// <param name="builder">The builder used to configure the VisitHistory entity type and its properties.</param>
        public void Configure(EntityTypeBuilder<VisitHistory> builder)
        {
            // Configure the table name and the primary key for the VisitHistory entity.
            builder.ToTable("visit_histories");
            builder.HasKey(v => v.Id);

            // Configure the VisitedOn property for the VisitHistory entity.
            builder.Property(v => v.VisitedOn).IsRequired();

            // A destination that was visited cannot be hard-deleted.
            builder.HasOne(v => v.Destination)
                .WithMany()
                .HasForeignKey(v => v.DestinationId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Loading the visits of a profile in order.
            builder.HasIndex(v => new { v.TouristProfileId, v.VisitedOn });

            // Finding who visited a destination.
            builder.HasIndex(v => v.DestinationId);
        }
    }
}
