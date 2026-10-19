using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.LocalInfo;

public sealed class LocalRuleTranslation : BaseEntity
{
    public Guid LocalRuleId { get; private set; }

    public string LanguageCode { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string Content { get; private set; } = string.Empty;

    private LocalRuleTranslation()
    {
    }

    private LocalRuleTranslation(
        Guid localRuleId,
        string languageCode,
        string title,
        string content)
    {
        LocalRuleId = localRuleId;
        LanguageCode = languageCode;
        Title = title;
        Content = content;
    }

    public static LocalRuleTranslation Create(
        Guid localRuleId,
        string languageCode,
        string title,
        string content)
    {
        if (localRuleId == Guid.Empty)
        {
            throw new ArgumentException(
                "LocalRuleId cannot be empty.",
                nameof(localRuleId));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(languageCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        return new LocalRuleTranslation(
            localRuleId,
            ValidateLanguageCode(languageCode),
            title.Trim(),
            content.Trim());
    }

    public void Update(string languageCode, string title, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(languageCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(content);

        LanguageCode = ValidateLanguageCode(languageCode);
        Title = title.Trim();
        Content = content.Trim();
    }

    private static string ValidateLanguageCode(string languageCode)
    {
        var normalizedCode = languageCode.Trim().ToLowerInvariant();

        if (normalizedCode.Length is < 2 or > 10 ||
            normalizedCode.Split('-').Any(part =>
                part.Length is < 2 or > 8 ||
                part.Any(c => !char.IsAsciiLetterOrDigit(c))))
        {
            throw new ArgumentException(
                "Invalid language code.",
                nameof(languageCode));
        }

        return normalizedCode;
    }
}