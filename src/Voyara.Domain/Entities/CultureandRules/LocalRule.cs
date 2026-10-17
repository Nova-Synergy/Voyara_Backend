using Voyara.Domain.Enums;
using Voyara.SharedKernel.Entities;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Culture_Rules;

/// <summary>
/// Aggregate root for a cultural, legal or behavioral rule that applies to a country, a city or a destination.
/// The localized texts of the rule are kept as <see cref="Translations"/>.
/// </summary>
/// <remarks>
/// <para>
/// The aggregate protects these invariants: an official rule states its source, a rule always has at least one
/// translation, and there is at most one translation per language. The scope of a rule never changes after
/// creation; a rule for another place is a new rule.
/// </para>
/// <para>
/// The effective rules of a destination are the rules of the destination, of its city and of its country.
/// The application layer builds those three <see cref="RuleScope"/>s and loads the rules through
/// <c>ILocalRuleRepository.ListByScopesAsync</c>.
/// </para>
/// </remarks>
public sealed class LocalRule : AggregateRoot
{
    /// <summary>Maximum length of <see cref="Source"/>.</summary>
    public const int MaxSourceLength = 500;

    private readonly List<LocalRuleTranslation> _translations = [];

    /// <summary>The category of the rule.</summary>
    public RuleType Type { get; private set; }

    /// <summary>Where the rule applies.</summary>
    public RuleScope Scope { get; private set; } = null!;

    /// <summary><see langword="true"/> when the rule comes from an official authority.</summary>
    public bool IsOfficial { get; private set; }

    /// <summary>Where the rule comes from (authority, law reference or website). Empty for unofficial rules without a source.</summary>
    public string Source { get; private set; } = string.Empty;

    /// <summary>UTC timestamp when the rule was last reviewed for accuracy.</summary>
    public DateTimeOffset LastReviewedAt { get; private set; }

    /// <summary>Localized texts of the rule, at most one per language and at least one in total.</summary>
    public IReadOnlyCollection<LocalRuleTranslation> Translations => _translations.AsReadOnly();

    private LocalRule()
    {
    }

    private LocalRule(RuleType type, RuleScope scope, bool isOfficial, string source, LocalRuleTranslation initialTranslation)
    {
        Type = type;
        Scope = scope;
        IsOfficial = isOfficial;
        Source = source;
        LastReviewedAt = DateTimeOffset.UtcNow;
        _translations.Add(initialTranslation);
    }

    /// <summary>Creates a rule with its first translation.</summary>
    public static Result<LocalRule> Create(
        RuleType type,
        RuleScope? scope,
        bool isOfficial,
        string? source,
        LocalRuleTranslation? initialTranslation)
    {
        if (!Enum.IsDefined(type))
        {
            return Result.Failure<LocalRule>(LocalRuleErrors.InvalidType);
        }

        if (scope is null)
        {
            return Result.Failure<LocalRule>(LocalRuleErrors.ScopeRequired);
        }

        var normalizedSource = NormalizeSource(isOfficial, source);
        if (normalizedSource.IsFailure)
        {
            return Result.Failure<LocalRule>(normalizedSource.Error);
        }

        if (initialTranslation is null)
        {
            return Result.Failure<LocalRule>(LocalRuleErrors.TranslationRequired);
        }

        return new LocalRule(type, scope, isOfficial, normalizedSource.Value, initialTranslation);
    }

    /// <summary>Changes the category, the official flag and the source of the rule.</summary>
    public Result<LocalRule> Update(RuleType type, bool isOfficial, string? source)
    {
        if (!Enum.IsDefined(type))
        {
            return Result.Failure<LocalRule>(LocalRuleErrors.InvalidType);
        }

        var normalizedSource = NormalizeSource(isOfficial, source);
        if (normalizedSource.IsFailure)
        {
            return Result.Failure<LocalRule>(normalizedSource.Error);
        }

        Type = type;
        IsOfficial = isOfficial;
        Source = normalizedSource.Value;
        return this;
    }

    /// <summary>Records that the rule was reviewed now.</summary>
    public void MarkAsReviewed() => LastReviewedAt = DateTimeOffset.UtcNow;

    /// <summary>Adds a translation for a language that has none yet.</summary>
    public Result<LocalRule> AddTranslation(LocalRuleTranslation? translation)
    {
        if (translation is null)
        {
            return Result.Failure<LocalRule>(LocalRuleErrors.TranslationRequired);
        }

        if (_translations.Any(t => SameLanguage(t.LanguageCode, translation.LanguageCode)))
        {
            return Result.Failure<LocalRule>(LocalRuleErrors.DuplicateTranslation);
        }

        _translations.Add(translation);
        return this;
    }

    /// <summary>Replaces the existing translation of the same language.</summary>
    public Result<LocalRule> ReplaceTranslation(LocalRuleTranslation? translation)
    {
        if (translation is null)
        {
            return Result.Failure<LocalRule>(LocalRuleErrors.TranslationRequired);
        }

        var index = _translations.FindIndex(t => SameLanguage(t.LanguageCode, translation.LanguageCode));
        if (index < 0)
        {
            return Result.Failure<LocalRule>(LocalRuleErrors.TranslationNotFound);
        }

        _translations[index] = translation;
        return this;
    }

    /// <summary>Removes the translation of a language. The last translation cannot be removed.</summary>
    public Result<LocalRule> RemoveTranslation(string? languageCode)
    {
        var index = _translations.FindIndex(t => SameLanguage(t.LanguageCode, languageCode));
        if (index < 0)
        {
            return Result.Failure<LocalRule>(LocalRuleErrors.TranslationNotFound);
        }

        if (_translations.Count == 1)
        {
            return Result.Failure<LocalRule>(LocalRuleErrors.CannotRemoveLastTranslation);
        }

        _translations.RemoveAt(index);
        return this;
    }

    private static bool SameLanguage(string left, string? right) =>
        string.Equals(left, right?.Trim(), StringComparison.OrdinalIgnoreCase);

    private static Result<string> NormalizeSource(bool isOfficial, string? source)
    {
        var normalized = source?.Trim() ?? string.Empty;

        if (isOfficial && normalized.Length == 0)
        {
            return Result.Failure<string>(LocalRuleErrors.OfficialRuleRequiresSource);
        }

        if (normalized.Length > MaxSourceLength)
        {
            return Result.Failure<string>(LocalRuleErrors.SourceTooLong);
        }

        return normalized;
    }
}
