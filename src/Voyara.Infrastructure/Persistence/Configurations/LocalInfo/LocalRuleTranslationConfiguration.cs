using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.LocalInfo;

namespace Voyara.Infrastructure.Persistence.Configurations.LocalInfo;

public sealed class LocalRuleTranslationConfiguration
    : IEntityTypeConfiguration<LocalRuleTranslation>
{
    public void Configure(EntityTypeBuilder<LocalRuleTranslation> builder)
    {
        builder.ToTable("local_info_rule_translations");

        builder.HasKey(translation => translation.Id);

        builder.Property(translation => translation.LocalRuleId)
            .IsRequired();

        builder.Property(translation => translation.LanguageCode)
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(translation => translation.Title)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(translation => translation.Content)
            .HasColumnType("text")
            .IsRequired();

        builder.HasIndex(translation => new
        {
            translation.LocalRuleId,
            translation.LanguageCode
        }).IsUnique();
    }
}