using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Transportation;

/// <summary>Domain errors raised by <see cref="TransportType"/>.</summary>
public static class TransportTypeErrors
{
    /// <summary>The identifier is not a positive number.</summary>
    public static readonly Error InvalidId = Error.Validation(
        "TransportType.InvalidId", "Transport type id must be a positive number.");

    /// <summary>The code is empty or too long.</summary>
    public static readonly Error InvalidCode = Error.Validation(
        "TransportType.InvalidCode", "Transport type code is required and must not exceed 50 characters.");

    /// <summary>The name is empty or too long.</summary>
    public static readonly Error InvalidName = Error.Validation(
        "TransportType.InvalidName", "Transport type name is required and must not exceed 100 characters.");
}
