using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Culture_Rules;

namespace Voyara.Infrastructure.Persistence.Configurations.Culture_Rules;

/// <summary>
/// Mapping for <see cref="RuleScope"/>, a value object owned by <see cref="LocalRule"/> and stored in the
/// <c>local_rules</c> table. It is applied from <see cref="LocalRuleConfiguration"/>.
/// </summary>
internal static class RuleScopeConfiguration
{
    /// <summary>Configures the scope columns.</summary>
    public static void Configure(OwnedNavigationBuilder<LocalRule, RuleScope> builder)
    {
        builder.Property(s => s.Type)
            .HasColumnName("Scope")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // The target is a country, a city or a destination, so it is referenced by Id only (no foreign key).
        builder.Property(s => s.TargetId)
            .HasColumnName("ScopeId")
            .HasMaxLength(50)
            .IsRequired();

        // Supports the most common query: all rules for a given scope.
        builder.HasIndex(s => new { s.Type, s.TargetId });
    }
}
