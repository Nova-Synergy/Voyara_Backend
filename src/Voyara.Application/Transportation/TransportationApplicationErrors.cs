using Voyara.SharedKernel.Results;

namespace Voyara.Application.Transportation;

/// <summary>Errors raised by the Transportation use cases (domain rules have their own errors).</summary>
internal static class TransportationApplicationErrors
{
    /// <summary>The city transport does not exist.</summary>
    public static Error CityTransportNotFound(Guid id)
        => Error.NotFound("CityTransport.NotFound", $"City transport '{id}' was not found.");

    /// <summary>The transport type does not exist.</summary>
    public static Error TransportTypeNotFound(int id)
        => Error.NotFound("TransportType.NotFound", $"Transport type '{id}' was not found.");

    /// <summary>The city already has this transport type.</summary>
    public static readonly Error CityTransportAlreadyExists = Error.Validation(
        "CityTransport.AlreadyExists", "A transport of this type already exists for the city.");
}
