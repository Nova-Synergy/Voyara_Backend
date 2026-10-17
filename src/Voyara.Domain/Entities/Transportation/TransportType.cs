using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Transportation;

/// <summary>
/// Reference data that describes a kind of transportation, such as metro, bus or taxi.
/// </summary>
/// <remarks>
/// Its own aggregate: other aggregates (for example <see cref="CityTransport"/>) refer to it by
/// <see cref="Id"/> only. Identifiers are assigned explicitly because the values are fixed reference data.
/// </remarks>
public sealed class TransportType
{
    /// <summary>Maximum length of <see cref="Code"/>.</summary>
    public const int MaxCodeLength = 50;

    /// <summary>Maximum length of <see cref="Name"/>.</summary>
    public const int MaxNameLength = 100;

    /// <summary>Identifier of the transport type.</summary>
    public int Id { get; private set; }

    /// <summary>Stable, unique machine-readable code in upper case (for example <c>METRO</c>).</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>Human-readable display name.</summary>
    public string Name { get; private set; } = string.Empty;

    private TransportType()
    {
    }

    private TransportType(int id, string code, string name)
    {
        Id = id;
        Code = code;
        Name = name;
    }

    /// <summary>Creates a transport type after validating its invariants.</summary>
    public static Result<TransportType> Create(int id, string? code, string? name)
    {
        if (id <= 0)
        {
            return Result.Failure<TransportType>(TransportTypeErrors.InvalidId);
        }

        var normalizedCode = code?.Trim().ToUpperInvariant() ?? string.Empty;
        if (normalizedCode.Length == 0 || normalizedCode.Length > MaxCodeLength)
        {
            return Result.Failure<TransportType>(TransportTypeErrors.InvalidCode);
        }

        var normalizedName = name?.Trim() ?? string.Empty;
        if (normalizedName.Length == 0 || normalizedName.Length > MaxNameLength)
        {
            return Result.Failure<TransportType>(TransportTypeErrors.InvalidName);
        }

        return new TransportType(id, normalizedCode, normalizedName);
    }

    /// <summary>Changes the display name.</summary>
    public Result<TransportType> Rename(string? name)
    {
        var normalizedName = name?.Trim() ?? string.Empty;
        if (normalizedName.Length == 0 || normalizedName.Length > MaxNameLength)
        {
            return Result.Failure<TransportType>(TransportTypeErrors.InvalidName);
        }

        Name = normalizedName;
        return this;
    }
}
