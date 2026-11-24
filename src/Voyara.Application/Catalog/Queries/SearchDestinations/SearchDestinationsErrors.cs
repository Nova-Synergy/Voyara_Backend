using Voyara.SharedKernel.Results;

namespace Voyara.Application.Catalog.Queries.SearchDestinations;

/// <summary>Errors returned by the destination search.</summary>
public static class SearchDestinationsErrors
{
    /// <summary>The requested page (or page size) is outside the supported range.</summary>
    public static readonly Error PageOutOfRange =
        Error.Validation("Destinations.PageOutOfRange", "The requested page is out of the supported range.");
}
