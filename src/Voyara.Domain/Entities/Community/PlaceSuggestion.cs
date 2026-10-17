
using Voyara.Domain.Enums;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.Community;

/// <summary>
/// Represents a tourist's proposal for a new tourism place.
/// </summary>
public sealed class PlaceSuggestion : AggregateRoot
{
    private readonly List<SuggestionPhoto> _photos = new();

    /// <summary>
    /// Identifier of the tourist who submitted the suggestion.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Proposed name of the place.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Description of the proposed place.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Geographical location of the proposed place.
    /// </summary>
    public GeoPoint Location { get; private set; } = null!;

    /// <summary>
    /// Reason for suggesting the place.
    /// </summary>
    public string Reason { get; private set; } = string.Empty;

    /// <summary>
    /// Current status of the suggestion.
    /// </summary>
    public SuggestionStatus Status { get; private set; } = SuggestionStatus.Pending;

    /// <summary>
    /// Explanation for the decision, if applicable.
    /// </summary>
    public string? DecisionReason { get; private set; }

    /// <summary>
    /// Identifier of the destination created from this suggestion, if applicable.
    /// </summary>
    public Guid? DestinationId { get; private set; }

    /// <summary>
    /// Photos attached to the suggestion.
    /// </summary>
    public IReadOnlyCollection<SuggestionPhoto> Photos => _photos.AsReadOnly();

    private PlaceSuggestion()
    {
    }
}
