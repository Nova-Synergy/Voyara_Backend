using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Guides;

namespace Voyara.Infrastructure.Persistence.Configurations.Guides;

/// <summary>
/// Maps <see cref="GuideDocument"/>. It is discovered automatically by ApplyConfigurationsFromAssembly.
/// The relationship has no navigation on <see cref="GuideProfile"/>, so the aggregate stays unchanged.
/// </summary>
public sealed class GuideDocumentConfiguration : IEntityTypeConfiguration<GuideDocument>
{
    public void Configure(EntityTypeBuilder<GuideDocument> builder)
    {
        builder.ToTable("guide_documents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentType).IsRequired().HasMaxLength(GuideDocument.MaxDocumentTypeLength);
        builder.Property(x => x.OriginalFileName).IsRequired().HasMaxLength(GuideDocument.MaxOriginalFileNameLength);
        builder.Property(x => x.StoredFileName).IsRequired().HasMaxLength(GuideDocument.MaxStoredFileNameLength);
        builder.Property(x => x.ContentType).IsRequired().HasMaxLength(GuideDocument.MaxContentTypeLength);
        builder.Property(x => x.SizeBytes).IsRequired();

        builder.HasOne<GuideProfile>()
            .WithMany()
            .HasForeignKey(x => x.GuideProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.GuideProfileId);
        builder.HasIndex(x => x.StoredFileName).IsUnique();

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
