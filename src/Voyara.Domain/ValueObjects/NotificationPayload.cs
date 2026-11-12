using System.Text.Json;
using Voyara.SharedKernel.Results;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.ValueObjects;

/// <summary>
/// Extra structured data attached to a notification, for example the id of the related review. It is always a
/// well-formed JSON object of at most <see cref="MaxLength"/> characters.
/// </summary>
public sealed class NotificationPayload : ValueObject
{
    /// <summary>Maximum allowed length of the JSON text.</summary>
    public const int MaxLength = 4000;

    /// <summary>The validated JSON object text.</summary>
    public string Json { get; private set; } = "{}";

    private NotificationPayload()
    {
    }

    private NotificationPayload(string json) => Json = json;

    /// <summary>Creates a payload, or fails when the text is empty, too long, invalid JSON or not an object.</summary>
    public static Result<NotificationPayload> Create(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Result.Failure<NotificationPayload>(Error.Validation(
                "NotificationPayload.Empty", "The payload cannot be empty."));
        }

        var trimmed = json.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<NotificationPayload>(Error.Validation(
                "NotificationPayload.TooLong", $"The payload can be at most {MaxLength} characters."));
        }

        try
        {
            using var document = JsonDocument.Parse(trimmed);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return Result.Failure<NotificationPayload>(Error.Validation(
                    "NotificationPayload.NotAnObject", "The payload must be a JSON object."));
            }
        }
        catch (JsonException)
        {
            return Result.Failure<NotificationPayload>(Error.Validation(
                "NotificationPayload.InvalidJson", "The payload must be valid JSON."));
        }

        return new NotificationPayload(trimmed);
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Json;
    }

    /// <inheritdoc />
    public override string ToString() => Json;
}