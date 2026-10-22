using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.LocalInfo;

namespace Voyara.Infrastructure.Persistence.Configurations.LocalInfo;

public sealed class EmbassyContactConfiguration
    : IEntityTypeConfiguration<EmbassyContact>
{
    public void Configure(EntityTypeBuilder<EmbassyContact> builder)
    {
        builder.ToTable("EmbassyContacts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.HostCountryCode)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(x => x.RepresentedNationalityCode)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(x => x.Address)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasIndex(x => new
        {
            x.HostCountryCode,
            x.RepresentedNationalityCode
        });
    }
}