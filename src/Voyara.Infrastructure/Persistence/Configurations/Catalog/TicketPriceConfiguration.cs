using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Catalog;

namespace Voyara.Infrastructure.Persistence.Configurations.Catalog
{
    /// <summary>
    /// Configuration for the TicketPrice entity type.
    /// The relationship with Destination is configured in DestinationConfiguration.
    /// </summary>
    public sealed class TicketPriceConfiguration : IEntityTypeConfiguration<TicketPrice>
    {
        /// <summary>
        /// Configures the entity type for the TicketPrice model using the specified builder.
        /// </summary>
        /// <remarks>This method sets up primary keys, property constraints, relationships, and indexes for the
        /// TicketPrice entity. It is typically called by the Entity Framework infrastructure when building the
        /// model.</remarks>
        /// <param name="builder">The builder used to configure the TicketPrice entity type and its properties.</param>
        public void Configure(EntityTypeBuilder<TicketPrice> builder)
        {
            // Configure the table name and the primary key for the TicketPrice entity.
            builder.ToTable("ticket_prices");
            builder.HasKey(p => p.Id);

            // Configure the VisitorType property; the enum is stored as text.
            builder.Property(p => p.VisitorType).HasConversion<string>().HasMaxLength(20).IsRequired();

            // Configure the Notes property for the TicketPrice entity.
            builder.Property(p => p.Notes).HasMaxLength(500).IsRequired();

            // Money value object flattened into PriceAmount / PriceCurrency columns.
            builder.ComplexProperty(p => p.Price, money =>
            {
                money.Property(m => m.Amount).HasColumnName("PriceAmount").HasPrecision(18, 2).IsRequired();
                money.Property(m => m.Currency).HasColumnName("PriceCurrency").HasMaxLength(3).IsFixedLength().IsRequired();
            });

            // One price per destination and visitor type.
            builder.HasIndex(p => new { p.DestinationId, p.VisitorType }).IsUnique();
        }
    }
}
