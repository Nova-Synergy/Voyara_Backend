namespace Voyara.SharedKernel.Results;

/// <summary>Describes an expected business failure.</summary>
public sealed record Error
{
    /// <summary>Stable machine-readable error code.</summary>
    public string Code { get; }

    /// <summary>Human-readable error description.</summary>
    public string Description { get; }

    /// <summary>Error classification used for HTTP mapping.</summary>
    public ErrorType Type { get; }

    /// <summary>Placeholder error used by successful results.</summary>
    public static Error None { get; } = new("General.None", string.Empty, ErrorType.Failure);

    private Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    /// <summary>Creates a validation error.</summary>
    public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);

    /// <summary>Creates a not-found error.</summary>
    public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);

    /// <summary>Creates an authorization error.</summary>
    public static Error Unauthorized(string code, string description) => new(code, description, ErrorType.Unauthorized);

    /// <summary>Creates a conflict error.</summary>
    public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);

    /// <summary>Creates a general failure error.</summary>
    public static Error Failure(string code, string description) => new(code, description, ErrorType.Failure);

    /// <inheritdoc />
    public override string ToString() => $"{Code}: {Description}";
}
