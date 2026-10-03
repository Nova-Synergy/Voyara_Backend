using Voyara.Application.Abstractions.Messaging;
using Voyara.Application.DTOs;

namespace Voyara.Application.Catalog.Queries.GetDestinationById;

/// <summary>Gets a single destination by identifier; cached under a stable key and invalidated by tag.</summary>
public sealed record GetDestinationByIdQuery(Guid DestinationId) : ICachedQuery<DestinationResponse>
{
    /// <inheritdoc />
    public string CacheKey => $"destinations:id:{DestinationId}";

    /// <inheritdoc />
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);

    /// <inheritdoc />
    public IReadOnlyCollection<string> Tags => new[] { "destinations" };
}
