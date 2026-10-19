using Voyara.Domain.Enums;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.LocalInfo;

public sealed class EmergencyContact : AuditableEntity
{
    public EmergencyType Type { get; private set; }

    public ScopeType Scope { get; private set; }

    public Guid ScopeId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string PhoneNumber { get; private set; } = string.Empty;

    public GeoPoint? Location { get; private set; }

    private EmergencyContact()
    {
    }
}