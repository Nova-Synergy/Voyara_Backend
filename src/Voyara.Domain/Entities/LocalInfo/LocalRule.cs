using Voyara.Domain.Entities.Culture_Rules;
using Voyara.Domain.Enums;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.LocalInfo;

public sealed class LocalRule : AuditableEntity
{
    public RuleType RuleType { get; private set; }

    public ScopeType ScopeType { get; private set; }

    public Guid ScopeId { get; private set; }

    public bool IsOfficial { get; private set; }

    public string Source { get; private set; } = string.Empty;

    public DateTimeOffset? LastReviewedAt { get; private set; }

    public ICollection<LocalRuleTranslation> Translations { get; private set; }
        = new List<LocalRuleTranslation>();

    private LocalRule()
    {
    }

    private LocalRule(
        RuleType ruleType,
        ScopeType scopeType,
        Guid scopeId,
        bool isOfficial,
        string source)
    {
        RuleType = ruleType;
        ScopeType = scopeType;
        ScopeId = scopeId;
        IsOfficial = isOfficial;
        Source = source;
    }

    public static LocalRule Create(
        RuleType ruleType,
        ScopeType scopeType,
        Guid scopeId,
        bool isOfficial,
        string source)
    {
        if (scopeId == Guid.Empty)
        {
            throw new ArgumentException(
                "ScopeId cannot be empty.",
                nameof(scopeId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(source);

        return new LocalRule(
            ruleType,
            scopeType,
            scopeId,
            isOfficial,
            source.Trim());
    }

    public void UpdateSource(string source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);

        Source = source.Trim();
    }

    public void MarkReviewed(DateTimeOffset reviewedAt)
    {
        if (reviewedAt > DateTimeOffset.UtcNow.AddMinutes(5))
        {
            throw new ArgumentOutOfRangeException(
                nameof(reviewedAt),
                "Review date cannot be in the future.");
        }

        LastReviewedAt = reviewedAt.ToUniversalTime();
    }

    public void AddTranslation(LocalRuleTranslation translation)
    {
        ArgumentNullException.ThrowIfNull(translation);

        if (Translations.Any(t =>
                string.Equals(
                    t.LanguageCode,
                    translation.LanguageCode,
                    StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "A translation for this language already exists.");
        }

        Translations.Add(translation);
    }

    public void RemoveTranslation(Guid translationId)
    {
        var translation = Translations
            .FirstOrDefault(t => t.Id == translationId);

        if (translation is null)
        {
            throw new InvalidOperationException(
                "Translation was not found.");
        }

        Translations.Remove(translation);
    }
}