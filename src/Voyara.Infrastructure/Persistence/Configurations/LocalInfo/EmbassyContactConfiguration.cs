using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voyara.Domain.Entities.LocalInfo;

namespace Voyara.Infrastructure.Persistence.Configurations.LocalInfo;

public sealed class EmbassyContactConfiguration
    : IEntityTypeConfiguration<EmbassyContact>
{
    public void Configure(EntityTypeBuilder<EmbassyContact> builder)
    {
        builder.ToTable("embassy_contacts");

        builder.HasKey(contact => contact.Id);

        builder.Property(contact => contact.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(contact => contact.HostCountryCode)
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(contact => contact.RepresentedNationalityCode)
            .HasMaxLength(2)
            .IsRequired();

        builder.Property(contact => contact.PhoneNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(contact => contact.Address)
            .HasMaxLength(500);

        builder.HasIndex(contact => new
        {
            contact.HostCountryCode,
            contact.RepresentedNationalityCode
        });

        builder.HasQueryFilter(contact => !contact.IsDeleted);
    }
}