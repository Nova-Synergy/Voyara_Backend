using Voyara.SharedKernel.Results;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.ValueObjects;

/// <summary>
/// The location of a stored photo. It is either an absolute http/https URL or a plain storage reference
/// such as "photos/a.jpg" (the file storage returns reference keys). Other schemes such as "javascript:"
/// or "file:" are rejected. The value is trimmed, contains no whitespace and is at most
/// <see cref="MaxLength"/> characters. Two photo URLs with the same value are equal.
/// </summary>
public sealed class PhotoUrl : ValueObject
{
    /// <summary>Maximum allowed length.</summary>
    public const int MaxLength = 2048;

    /// <summary>The validated, trimmed URL or storage reference.</summary>
    public string Value { get; private set; } = string.Empty;

    private PhotoUrl()
    {
    }

    private PhotoUrl(string value) => Value = value;

    /// <summary>Creates a photo URL, or fails with the first rule that is broken.</summary>
    public static Result<PhotoUrl> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<PhotoUrl>(Error.Validation(
                "PhotoUrl.Empty", "Photo URL cannot be empty."));
        }

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
        {
            return Result.Failure<PhotoUrl>(Error.Validation(
                "PhotoUrl.TooLong", $"Photo URL can be at most {MaxLength} characters."));
        }

        if (trimmed.Any(c => char.IsWhiteSpace(c) || char.IsControl(c)))
        {
            return Result.Failure<PhotoUrl>(Error.Validation(
                "PhotoUrl.Invalid", "Photo URL cannot contain whitespace or control characters."));
        }

        var isWebUrl = trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

        if (trimmed.Contains(':') && !isWebUrl)
        {
            return Result.Failure<PhotoUrl>(Error.Validation(
                "PhotoUrl.Invalid", "Only http/https URLs or plain storage references are allowed."));
        }

        return new PhotoUrl(trimmed);
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}