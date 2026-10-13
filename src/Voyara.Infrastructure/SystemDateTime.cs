using Voyara.Application.Abstractions;

namespace Voyara.Infrastructure;

/// <summary>Production clock implementation.</summary>
public sealed class SystemDateTime : IDateTime
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
