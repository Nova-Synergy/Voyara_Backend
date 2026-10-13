namespace Voyara.Application.Abstractions;

/// <summary>Clock abstraction to keep application logic testable.</summary>
public interface IDateTime
{
    /// <summary>Current UTC time.</summary>
    DateTimeOffset UtcNow { get; }
}
