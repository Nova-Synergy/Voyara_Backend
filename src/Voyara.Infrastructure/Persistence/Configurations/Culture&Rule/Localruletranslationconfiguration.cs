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
    /// EF Core Fluent API mapping for <see cref="LocalRuleTranslation"/>.
    /// </summary>
    public class LocalRuleTranslationConfiguration : IEntityTypeConfiguration<LocalRuleTranslation>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<LocalRuleTranslation> builder)
        {
            builder.ToTable("LocalRuleTranslations");

            // Composite key: one translation per language for each rule.
            builder.HasKey(x => new { x.LocalRuleId, x.LanguageCode });

            builder.Property(x => x.LanguageCode).IsRequired().HasMaxLength(10);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Content).IsRequired().HasMaxLength(4000);
        }
    }
}
