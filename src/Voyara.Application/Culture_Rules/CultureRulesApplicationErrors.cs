using Voyara.SharedKernel.Results;

namespace Voyara.Application.Culture_Rules;

/// <summary>Errors raised by the Culture &amp; Rules use cases (domain rules have their own errors).</summary>
internal static class CultureRulesApplicationErrors
{
    /// <summary>The local rule does not exist.</summary>
    public static Error LocalRuleNotFound(Guid id)
        => Error.NotFound("LocalRule.NotFound", $"Local rule '{id}' was not found.");

    /// <summary>The destination targeted by a rule does not exist.</summary>
    public static Error DestinationNotFound(Guid id)
        => Error.NotFound("Destination.NotFound", $"Destination '{id}' was not found.");
}
