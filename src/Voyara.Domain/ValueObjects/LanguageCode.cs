using Voyara.SharedKernel.Results;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.ValueObjects;

/// <summary>
/// A language identifier such as "en", "ar" or "pt-BR" (at most five characters, matching the template table).
/// Input is normalized (language in lowercase, region in uppercase) so "EN", "en" and " en " are the same code.
/// English is the fallback language of the platform.
/// </summary>
public sealed class LanguageCode : ValueObject
{
    /// <summary>The platform's fallback language.</summary>
    public static LanguageCode English { get; } = new("en");

    /// <summary>The normalized code, for example "en" or "pt-BR".</summary>
    public string Value { get; private set; } = string.Empty;

    /// <summary>
    /// The language without the region: "pt-BR" gives "pt", and "en" stays "en". Lets callers fall back from a
    /// regional variant to its base language and then to <see cref="English"/>.
    /// </summary>
    public LanguageCode Primary => Value.Length == 2 ? this : new LanguageCode(Value[..2]);

    private LanguageCode()
    {
    }

    private LanguageCode(string value) => Value = value;

    /// <summary>Creates a language code, or fails when it is empty or not in the form "xx" or "xx-XX".</summary>
    public static Result<LanguageCode> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<LanguageCode>(Error.Validation(
                "LanguageCode.Empty", "The language code is required."));
        }

        var parts = value.Trim().Split('-');

        var isValid = parts.Length is 1 or 2
            && parts[0].Length == 2
            && parts[0].All(char.IsAsciiLetter)
            && (parts.Length == 1 || (parts[1].Length == 2 && parts[1].All(char.IsAsciiLetter)));

        if (!isValid)
        {
            return Result.Failure<LanguageCode>(Error.Validation(
                "LanguageCode.Invalid", "The language code must look like 'en' or 'pt-BR'."));
        }

        var normalized = parts.Length == 1
            ? parts[0].ToLowerInvariant()
            : $"{parts[0].ToLowerInvariant()}-{parts[1].ToUpperInvariant()}";

        return new LanguageCode(normalized);
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}