using System.Text.Json;
using Voyara.SharedKernel.Results;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.ValueObjects;

/// <summary>
/// The extra information stored with an audit entry. It is always a well-formed JSON object of at most
/// <see cref="MaxLength"/> characters, and defaults to an empty object. Modeling it as a value object means
/// malformed details can never be written to the audit trail.
/// </summary>
public sealed class AuditDetails : ValueObject
{
    /// <summary>Maximum allowed length of the JSON text.</summary>
    public const int MaxLength = 8000;

    /// <summary>The JSON text used when no details are supplied.</summary>
    public const string EmptyJson = "{}";

    /// <summary>The validated JSON object text.</summary>
    public string Json { get; private set; } = EmptyJson;

    private AuditDetails()
    {
    }

    private AuditDetails(string json) => Json = json;

    /// <summary>
    /// Creates details from JSON text. Null or blank text gives empty details. Fails when the text is too
    /// long, is not valid JSON, or is valid JSON that is not an object.
    /// </summary>
    public static Result<AuditDetails> Create(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new AuditDetails(EmptyJson);
        }

        var trimmed = json.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<AuditDetails>(Error.Validation(
                "AuditDetails.TooLong", $"Audit details can be at most {MaxLength} characters."));
        }

        try
        {
            using var document = JsonDocument.Parse(trimmed);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return Result.Failure<AuditDetails>(Error.Validation(
                    "AuditDetails.NotAnObject", "Audit details must be a JSON object."));
            }
        }
        catch (JsonException)
        {
            return Result.Failure<AuditDetails>(Error.Validation(
                "AuditDetails.InvalidJson", "Audit details must be valid JSON."));
        }

        return new AuditDetails(trimmed);
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Json;
    }

    /// <inheritdoc />
    public override string ToString() => Json;
}