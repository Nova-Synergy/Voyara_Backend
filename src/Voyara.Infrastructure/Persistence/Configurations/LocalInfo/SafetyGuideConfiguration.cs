using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.LocalInfo;

namespace Voyara.Infrastructure.Persistence.Configurations.LocalInfo;

public sealed class SafetyGuideConfiguration
    : IEntityTypeConfiguration<SafetyGuide>
{
    public void Configure(EntityTypeBuilder<SafetyGuide> builder)
    {
        builder.ToTable("safety_guides");

        builder.HasKey(guide => guide.Id);

        builder.Property(guide => guide.Title)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(guide => guide.Steps)
            .HasConversion(
                steps => JsonSerializer.Serialize(steps, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<List<string>>(
                    json,
                    (JsonSerializerOptions?)null) ?? new List<string>())
            .HasColumnType("jsonb")
            .IsRequired();

        builder.HasQueryFilter(guide => !guide.IsDeleted);

        builder.HasIndex(guide => guide.Title);
    }
}