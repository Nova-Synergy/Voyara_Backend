using Voyara.SharedKernel.Results;
using Voyara.SharedKernel.ValueObjects;

namespace Voyara.Domain.ValueObjects;

/// <summary>
/// The text of a notification: a title and a body. Templates produce one when they are rendered and
/// notifications are created from one, so a message that is empty or too long can never reach a user.
/// </summary>
public sealed class NotificationMessage : ValueObject
{
    /// <summary>Maximum length of the title.</summary>
    public const int MaxTitleLength = 200;

    /// <summary>Maximum length of the body.</summary>
    public const int MaxBodyLength = 4000;

    /// <summary>The short headline.</summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>The full message text.</summary>
    public string Body { get; private set; } = string.Empty;

    private NotificationMessage()
    {
    }

    private NotificationMessage(string title, string body)
    {
        Title = title;
        Body = body;
    }

    /// <summary>Creates a message, or fails with the first rule that is broken.</summary>
    public static Result<NotificationMessage> Create(string? title, string? body)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<NotificationMessage>(Error.Validation(
                "NotificationMessage.TitleRequired", "The title is required."));
        }

        var trimmedTitle = title.Trim();
        if (trimmedTitle.Length > MaxTitleLength)
        {
            return Result.Failure<NotificationMessage>(Error.Validation(
                "NotificationMessage.TitleTooLong", $"The title can be at most {MaxTitleLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            return Result.Failure<NotificationMessage>(Error.Validation(
                "NotificationMessage.BodyRequired", "The body is required."));
        }

        var trimmedBody = body.Trim();
        if (trimmedBody.Length > MaxBodyLength)
        {
            return Result.Failure<NotificationMessage>(Error.Validation(
                "NotificationMessage.BodyTooLong", $"The body can be at most {MaxBodyLength} characters."));
        }

        return new NotificationMessage(trimmedTitle, trimmedBody);
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Title;
        yield return Body;
    }
}