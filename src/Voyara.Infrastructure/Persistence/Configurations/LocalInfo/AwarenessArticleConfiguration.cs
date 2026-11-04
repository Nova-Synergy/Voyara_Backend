using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.LocalInfo;

namespace Voyara.Infrastructure.Persistence.Configurations.LocalInfo;

public sealed class AwarenessArticleConfiguration
    : IEntityTypeConfiguration<AwarenessArticle>
{
    public void Configure(EntityTypeBuilder<AwarenessArticle> builder)
    {
        builder.ToTable("awareness_articles");

        builder.HasKey(article => article.Id);

        builder.Property(article => article.Id)
            .ValueGeneratedNever();

        builder.Property(article => article.Title)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(article => article.Body)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(article => article.Topic)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(article => article.Source)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(article => article.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(article => article.LastReviewedAt);

        builder.HasIndex(article => article.Topic);

        builder.HasIndex(article => article.Status);

        builder.HasQueryFilter(article => !article.IsDeleted);
    }
}