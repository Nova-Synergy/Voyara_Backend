using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.LocalInfo;

namespace Voyara.Infrastructure.Persistence.Configurations.LocalInfo;

public sealed class AwarenessArticleConfiguration
    : IEntityTypeConfiguration<AwarenessArticle>
{
    public void Configure(EntityTypeBuilder<AwarenessArticle> builder)
    {
        builder.ToTable("AwarenessArticles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Body)
            .IsRequired()
            .HasMaxLength(10000);

        builder.Property(x => x.Topic)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Source)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.LastReviewedAt)
            .IsRequired(false);

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Topic);
    }
}