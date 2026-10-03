using Voyara.Domain.Enums;
using Voyara.Domain.Events.Catalog;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Catalog;

/// <summary>
/// Aggregate root representing a tourist destination in the catalog.
/// </summary>
public sealed class Destination : AggregateRoot
{
    /// <summary>Display name of the destination.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Short marketing description.</summary>
    public string? Description { get; private set; }

    /// <summary>Geographic location of the destination.</summary>
    public GeoPoint Location { get; private set; } = null!;

    /// <summary>Current lifecycle status.</summary>
    public DestinationStatus Status { get; private set; } = DestinationStatus.Draft;

    private Destination()
    {
    }

    private Destination(string name, string? description, GeoPoint location)
    {
        Name = name;
        Description = description;
        Location = location;
        Status = DestinationStatus.Draft;
    }

    /// <summary>Creates a destination in the Draft status.</summary>
    public static Destination Create(string name, string? description, GeoPoint location)
        => new(name, description, location);

    /// <summary>Transitions the destination to Published, raising a domain event on first publication.</summary>
    public void Publish()
    {
        if (Status != DestinationStatus.Draft)
        {
            return;
        }

        Status = DestinationStatus.Published;
        RaiseDomainEvent(new DestinationPublishedDomainEvent(Guid.NewGuid(), DateTimeOffset.UtcNow, Id, Name));
    }

    /// <summary>Transitions the destination to Archived. Archived destinations are read-only.</summary>
    public void Archive()
    {
        if (Status == DestinationStatus.Archived)
        {
            return;
        }

        Status = DestinationStatus.Archived;
    }
}
