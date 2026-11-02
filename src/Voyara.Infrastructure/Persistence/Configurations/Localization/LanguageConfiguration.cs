using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.Localization;

namespace Voyara.Infrastructure.Persistence.Configurations.Localization;

public sealed class LanguageConfiguration
    : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("languages");

        builder.HasKey(l => l.Code);

        builder.Property(l => l.Code)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(l => l.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(l => l.IsRtl)
            .IsRequired();
    }
}