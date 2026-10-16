using Voyara.SharedKernel.Entities;

namespace Voyara.Domain.Entities.LocalInfo;

public sealed class SafetyGuide : AuditableEntity
{
    public string Title { get; private set; } = string.Empty;

    public List<string> Steps { get; private set; } = new();

    private SafetyGuide()
    {
    }
}