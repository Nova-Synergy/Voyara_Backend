using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Entities.Culture_Rules;

namespace Voyara.Infrastructure.Persistence.Configurations.Culture_Rule
{

    /// <summary>
    /// EF Core Fluent API mapping for <see cref="LocalRule"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="LocalRule.ScopeId"/> points to a country, city or destination depending on
    /// <see cref="LocalRule.Scope"/>, so it is mapped as a plain indexed column without a foreign key constraint.
    /// </remarks>
    public sealed class LocalRuleConfiguration : IEntityTypeConfiguration<LocalRule>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<LocalRule> builder)
        {
            builder.ToTable("local_rules");

            builder.HasKey(x => x.Id);

            // Enums are stored as text so the table stays readable and survives reordering of enum members.
            builder.Property(x => x.Type)
                   .HasConversion<string>()
                   .HasMaxLength(30)
                   .IsRequired();

            builder.Property(x => x.Scope)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(x => x.ScopeId).IsRequired().HasMaxLength(50);
            builder.Property(x => x.IsOfficial).IsRequired();
            builder.Property(x => x.Source).IsRequired().HasMaxLength(500);
            builder.Property(x => x.LastReviewedAt).IsRequired();

            // Supports the most common query: all rules for a given scope (for example, a city).
            builder.HasIndex(x => new { x.Scope, x.ScopeId });

            // Composition: LocalRule 1 --- 1..* LocalRuleTranslation.
            builder.HasMany(x => x.Translations)
                   .WithOne(t => t.LocalRule)
                   .HasForeignKey(t => t.LocalRuleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
