using Voyara.SharedKernel.Results;

namespace Voyara.Application.Transportation;

/// <summary>Application-level errors of the transportation module (F-20).</summary>
public static class TransportationErrors
{
    /// <summary>The city transport does not exist or was deleted.</summary>
    public static readonly Error NotFound =
        Error.NotFound("Transportation.NotFound", "The transportation record was not found.");

    /// <summary>The referenced transport type does not exist.</summary>
    public static readonly Error TransportTypeNotFound =
        Error.Validation("Transportation.TransportTypeNotFound", "The transport type does not exist.");

    /// <summary>A record already exists for the same city and transport type.</summary>
    public static readonly Error AlreadyExists =
        Error.Conflict("Transportation.AlreadyExists", "This transport type is already defined for the city.");
}
