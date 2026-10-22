using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Catalog;

namespace Voyara.Infrastructure.Persistence.Configurations.Catalog
{
    /// <summary>
    /// Configuration for the Category entity type.
    /// </summary>
    public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        /// <summary>
        /// Configures the entity type for the Category model using the specified builder.
        /// </summary>
        /// <remarks>This method sets up primary keys, property constraints, relationships, and indexes for the
        /// Category entity. It is typically called by the Entity Framework infrastructure when building the
        /// model.</remarks>
        /// <param name="builder">The builder used to configure the Category entity type and its properties.</param>
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // Configure the table name and the primary key for the Category entity.
            builder.ToTable("categories");
            builder.HasKey(c => c.Id);

            // Configure the Code property for the Category entity.
            builder.Property(c => c.Code).HasMaxLength(50).IsRequired();

            // Configure the Icon property for the Category entity.
            builder.Property(c => c.Icon).HasMaxLength(200).IsRequired();

            // Configure the IsActive property for the Category entity.
            builder.Property(c => c.IsActive).IsRequired();

            // Category codes are unique.
            builder.HasIndex(c => c.Code).IsUnique();
        }
    }
}
