using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Voyara.Domain.Enums;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Entities;
using Voyara.SharedKernel.Results;

namespace Voyara.Domain.Entities.Notifications;

/// <summary>
/// The reusable, translated text for a type of notification. Templates contain placeholders such as
/// <c>{UserName}</c> that <see cref="Render"/> fills in. New languages are new templates, so no schema change
/// is needed, and the platform falls back to English when a language is missing.
/// </summary>
/// <remarks>
/// Identity rule: the <c>Id</c> is derived from (type, language) by <see cref="ComputeId"/>, matching the
/// composite primary key of the design, so there is exactly one template per type and language.
/// </remarks>
public sealed class NotificationTemplate : AuditableEntity
{
    private static readonly Regex PlaceholderPattern =
        new(@"\{([A-Za-z][A-Za-z0-9_]*)\}", RegexOptions.CultureInvariant);

    /// <summary>The notification type this template is used for.</summary>
    public NotificationType Type { get; private set; }

    /// <summary>The language of the text.</summary>
    public LanguageCode LanguageCode { get; private set; } = null!;

    /// <summary>Subject (title) text, possibly containing <c>{placeholders}</c>.</summary>
    public string Subject { get; private set; } = string.Empty;

    /// <summary>Body text, possibly containing <c>{placeholders}</c>.</summary>
    public string Body { get; private set; } = string.Empty;

    private NotificationTemplate()
    {
    }

    private NotificationTemplate(NotificationType type, LanguageCode languageCode, NotificationMessage text)
    {
        Id = ComputeId(type, languageCode);
        Type = type;
        LanguageCode = languageCode;
        Subject = text.Title;
        Body = text.Body;
    }

    /// <summary>Computes the identity of the template for a notification type and language.</summary>
    public static Guid ComputeId(NotificationType type, LanguageCode languageCode)
    {
        var key = $"{(int)type}|{languageCode.Value}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        return new Guid(hash.AsSpan(0, 16));
    }

    /// <summary>Creates a template after validating the type, language and text.</summary>
    /// <returns>The new template, or the first rule that was broken.</returns>
    public static Result<NotificationTemplate> Create(
        NotificationType type, string languageCode, string subject, string body)
    {
        if (!Enum.IsDefined(type))
        {
            return Result.Failure<NotificationTemplate>(NotificationErrors.InvalidType);
        }

        var languageResult = LanguageCode.Create(languageCode);
        if (languageResult.IsFailure)
        {
            return Result.Failure<NotificationTemplate>(languageResult.Error);
        }

        var textResult = NotificationMessage.Create(subject, body);
        if (textResult.IsFailure)
        {
            return Result.Failure<NotificationTemplate>(textResult.Error);
        }

        return new NotificationTemplate(type, languageResult.Value, textResult.Value);
    }

    /// <summary>Replaces the template text (used when an admin edits a template).</summary>
    public Result Update(string subject, string body)
    {
        var textResult = NotificationMessage.Create(subject, body);
        if (textResult.IsFailure)
        {
            return Result.Failure(textResult.Error);
        }

        Subject = textResult.Value.Title;
        Body = textResult.Value.Body;
        return Result.Success();
    }

    /// <summary>
    /// Fills in the placeholders and returns the final message. With body <c>"Hi {UserName}"</c> and
    /// <c>UserName = "Sara"</c> the result is <c>"Hi Sara"</c>. Fails when a placeholder has no value (so raw
    /// <c>{placeholders}</c> never reach a user) or when the rendered text breaks the message rules. Values are
    /// inserted literally and are never expanded again.
    /// </summary>
    /// <param name="values">Placeholder names (without braces) mapped to their values; extra values are ignored.</param>
    public Result<NotificationMessage> Render(IReadOnlyDictionary<string, string> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var missing = PlaceholderPattern.Matches(Subject)
            .Concat(PlaceholderPattern.Matches(Body))
            .Select(match => match.Groups[1].Value)
            .FirstOrDefault(name => !values.ContainsKey(name));

        if (missing is not null)
        {
            return Result.Failure<NotificationMessage>(NotificationTemplateErrors.MissingValue(missing));
        }

        string Fill(string text) =>
            PlaceholderPattern.Replace(text, match => values[match.Groups[1].Value]);

        return NotificationMessage.Create(Fill(Subject), Fill(Body));
    }
}