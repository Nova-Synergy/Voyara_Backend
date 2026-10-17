using System.Text.RegularExpressions;
using Voyara.SharedKernel.Results;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.Entities.Culture_Rules;

/// <summary>
/// Value object with the text of a <see cref="LocalRule"/> in one language. A rule has at most one translation
/// per language, so the language code identifies it inside the rule.
/// </summary>
public sealed class LocalRuleTranslation : ValueObject
{
    /// <summary>Maximum length of <see cref="LanguageCode"/>.</summary>
    public const int MaxLanguageCodeLength = 10;

    /// <summary>Maximum length of <see cref="Title"/>.</summary>
    public const int MaxTitleLength = 200;

    /// <summary>Maximum length of <see cref="Content"/>.</summary>
    public const int MaxContentLength = 4000;

    private static readonly Regex LanguageCodePattern = new("^[A-Za-z]{2,3}(-[A-Za-z0-9]{2,4})?$", RegexOptions.Compiled);

    /// <summary>Language code, such as <c>ar</c>, <c>en</c> or <c>ar-EG</c>.</summary>
    public string LanguageCode { get; private set; } = string.Empty;

    /// <summary>Short title of the rule in this language.</summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>Full description of the rule in this language.</summary>
    public string Content { get; private set; } = string.Empty;

    private LocalRuleTranslation()
    {
    }

    private LocalRuleTranslation(string languageCode, string title, string content)
    {
        LanguageCode = languageCode;
        Title = title;
        Content = content;
    }

    /// <summary>Creates a validated translation.</summary>
    public static Result<LocalRuleTranslation> Create(string? languageCode, string? title, string? content)
    {
        var normalizedCode = languageCode?.Trim() ?? string.Empty;
        if (normalizedCode.Length == 0
            || normalizedCode.Length > MaxLanguageCodeLength
            || !LanguageCodePattern.IsMatch(normalizedCode))
        {
            return Result.Failure<LocalRuleTranslation>(LocalRuleErrors.InvalidLanguageCode);
        }

        var normalizedTitle = title?.Trim() ?? string.Empty;
        if (normalizedTitle.Length == 0 || normalizedTitle.Length > MaxTitleLength)
        {
            return Result.Failure<LocalRuleTranslation>(LocalRuleErrors.InvalidTitle);
        }

        var normalizedContent = content?.Trim() ?? string.Empty;
        if (normalizedContent.Length == 0 || normalizedContent.Length > MaxContentLength)
        {
            return Result.Failure<LocalRuleTranslation>(LocalRuleErrors.InvalidContent);
        }

        return new LocalRuleTranslation(normalizedCode, normalizedTitle, normalizedContent);
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return LanguageCode.ToLowerInvariant();
        yield return Title;
        yield return Content;
    }
}
