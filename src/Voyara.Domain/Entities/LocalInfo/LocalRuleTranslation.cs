using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.LocalInfo;

public sealed class LocalRuleTranslation : BaseEntity
{
    public Guid LocalRuleId { get; private set; }

    public string LanguageCode { get; private set; }
        = string.Empty;

    public string Title { get; private set; }
        = string.Empty;

    public string Content { get; private set; }
        = string.Empty;

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
            throw new ArgumentException(
                "LocalRuleId cannot be empty.");

        if (string.IsNullOrWhiteSpace(languageCode))
            throw new ArgumentException(
                "Language code is required.");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Title is required.");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException(
                "Content is required.");

        return new LocalRuleTranslation(
            localRuleId,
            languageCode.Trim(),
            title.Trim(),
            content.Trim());
    }
}