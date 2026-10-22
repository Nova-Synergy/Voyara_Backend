using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.LocalInfo;

namespace Voyara.Infrastructure.Persistence.Configurations.LocalInfo;

public sealed class SafetyGuideConfiguration
    : IEntityTypeConfiguration<SafetyGuide>
{
    public void Configure(EntityTypeBuilder<SafetyGuide> builder)
    {
        builder.ToTable("SafetyGuides");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Steps)
            .IsRequired()
            .HasConversion(
                steps => string.Join("\n", steps),
                value => value.Split(
                    '\n',
                    StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasColumnType("nvarchar(max)");
    }
}