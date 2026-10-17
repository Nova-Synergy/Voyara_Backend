using Voyara.SharedKernel.Results;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.Entities.Transportation;

/// <summary>
/// Value object that states a <see cref="CityTransport"/> reaches a destination. The destination belongs to
/// the Catalog aggregate, so it is referenced by <see cref="DestinationId"/> only.
/// </summary>
public sealed class DestinationTransport : ValueObject
{
    /// <summary>Identifier of the destination that can be reached.</summary>
    public Guid DestinationId { get; private set; }

    private DestinationTransport()
    {
    }

    private DestinationTransport(Guid destinationId)
    {
        DestinationId = destinationId;
    }

    internal static Result<DestinationTransport> Create(Guid destinationId)
    {
        if (destinationId == Guid.Empty)
        {
            return Result.Failure<DestinationTransport>(CityTransportErrors.InvalidDestinationId);
        }

        return new DestinationTransport(destinationId);
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return DestinationId;
    }
}
