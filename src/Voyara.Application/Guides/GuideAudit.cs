using System.Text.Json;
using Voyara.Domain.Entities.Moderation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides;

/// <summary>Builds audit log entries for administrator actions on guide profiles (UC-06).</summary>
internal static class GuideAudit
{
    public const string TargetType = "GuideProfile";

    public static Result<AuditLog> Create(Guid actorId, string action, Guid profileId, object? details = null)
    {
        var json = details is null ? null : JsonSerializer.Serialize(details);
        return AuditLog.Create(actorId, action, TargetType, profileId.ToString(), json);
    }
}
