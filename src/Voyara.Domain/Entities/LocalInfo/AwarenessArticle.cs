using Voyara.Domain.Enums;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.LocalInfo;

public sealed class AwarenessArticle : AuditableEntity
{
    public string Title { get; private set; } = string.Empty;

    public string Body { get; private set; } = string.Empty;

    public string Topic { get; private set; } = string.Empty;

    public string Source { get; private set; } = string.Empty;

    public ContentStatus Status { get; private set; } = ContentStatus.Draft;

    public DateTimeOffset? LastReviewedAt { get; private set; }

    private AwarenessArticle()
    {
    }

    private AwarenessArticle(
        string title,
        string body,
        string topic,
        string source)
    {
        Title = title;
        Body = body;
        Topic = topic;
        Source = source;
        Status = ContentStatus.Draft;
    }

    public static AwarenessArticle Create(
        string title,
        string body,
        string topic,
        string source)
    {
        ValidateRequiredFields(title, body, topic, source);

        return new AwarenessArticle(
            title.Trim(),
            body.Trim(),
            topic.Trim(),
            source.Trim());
    }

    public void Update(
        string title,
        string body,
        string topic,
        string source)
    {
        EnsureNotArchived();
        ValidateRequiredFields(title, body, topic, source);

        Title = title.Trim();
        Body = body.Trim();
        Topic = topic.Trim();
        Source = source.Trim();
    }

    public void MarkAsReviewed(DateTimeOffset reviewedAt)
    {
        EnsureNotArchived();

        if (reviewedAt > DateTimeOffset.UtcNow.AddMinutes(5))
        {
            throw new ArgumentOutOfRangeException(
                nameof(reviewedAt),
                "Review date cannot be in the future.");
        }

        LastReviewedAt = reviewedAt.ToUniversalTime();
    }

    public void Publish()
    {
        EnsureNotArchived();

        if (LastReviewedAt is null)
        {
            throw new InvalidOperationException(
                "The article must be reviewed before publishing.");
        }

        Status = ContentStatus.Published;
    }

    public void Archive()
    {
        Status = ContentStatus.Archived;
    }

    private void EnsureNotArchived()
    {
        if (Status == ContentStatus.Archived)
        {
            throw new InvalidOperationException(
                "An archived article cannot be modified.");
        }
    }

    private static void ValidateRequiredFields(
        string title,
        string body,
        string topic,
        string source)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);
        ArgumentException.ThrowIfNullOrWhiteSpace(topic);
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
    }
}