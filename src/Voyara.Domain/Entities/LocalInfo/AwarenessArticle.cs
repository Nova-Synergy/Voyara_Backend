using Voyara.Domain.Enums;
using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.LocalInfo;

public sealed class AwarenessArticle : AuditableEntity
{
    public string Title { get; private set; } = string.Empty;

    public string Body { get; private set; } = string.Empty;

    public string Topic { get; private set; } = string.Empty;

    public string Source { get; private set; } = string.Empty;

    public ContentStatus Status { get; private set; }

    public DateTimeOffset? LastReviewedAt { get; private set; }

    private AwarenessArticle()
    {
    }
}