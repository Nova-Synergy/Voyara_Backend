using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Culture_Rules;

namespace Voyara.Infrastructure.Persistence.Configurations.Culture_Rules;

/// <summary>
/// Mapping for <see cref="LocalRuleTranslation"/>, a value object owned by <see cref="LocalRule"/>.
/// It is applied from <see cref="LocalRuleConfiguration"/>.
/// </summary>
internal static class LocalRuleTranslationConfiguration
{
    /// <summary>Configures the owned translation table.</summary>
    public static void Configure(OwnedNavigationBuilder<LocalRule, LocalRuleTranslation> builder)
    {
        builder.ToTable("local_rule_translations");
        builder.WithOwner().HasForeignKey("LocalRuleId");

        // One translation per language for each rule.
        builder.HasKey("LocalRuleId", nameof(LocalRuleTranslation.LanguageCode));

        builder.Property(t => t.LanguageCode)
            .HasMaxLength(LocalRuleTranslation.MaxLanguageCodeLength)
            .IsRequired();
        builder.Property(t => t.Title)
            .HasMaxLength(LocalRuleTranslation.MaxTitleLength)
            .IsRequired();
        builder.Property(t => t.Content)
            .HasMaxLength(LocalRuleTranslation.MaxContentLength)
            .IsRequired();
    }
}
