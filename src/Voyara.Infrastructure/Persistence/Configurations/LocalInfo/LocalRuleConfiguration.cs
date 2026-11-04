using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.LocalInfo;

namespace Voyara.Infrastructure.Persistence.Configurations.LocalInfo;

public sealed class LocalRuleConfiguration
    : IEntityTypeConfiguration<LocalRule>
{
    public void Configure(EntityTypeBuilder<LocalRule> builder)
    {
        builder.ToTable("local_info_rules");

        builder.HasKey(rule => rule.Id);

        builder.Property(rule => rule.RuleType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(rule => rule.ScopeType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(rule => rule.ScopeId)
            .IsRequired();

        builder.Property(rule => rule.IsOfficial)
            .IsRequired();

        builder.Property(rule => rule.Source)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(rule => rule.LastReviewedAt);

        builder.HasMany(rule => rule.Translations)
            .WithOne()
            .HasForeignKey(translation => translation.LocalRuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(rule => new
        {
            rule.ScopeType,
            rule.ScopeId
        });

        builder.HasQueryFilter(rule => !rule.IsDeleted);
    }
}