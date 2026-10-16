using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.LocalInfo;

public sealed class EmbassyContact : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;

    public string HostCountryCode { get; private set; }
        = string.Empty;

    public string RepresentedNationalityCode { get; private set; }
        = string.Empty;

    public string PhoneNumber { get; private set; }
        = string.Empty;

    public string? Address { get; private set; }

    private EmbassyContact()
    {
    }
}