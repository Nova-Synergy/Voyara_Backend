using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides;

/// <summary>
/// Runs aggregate operations that signal rule violations with exceptions and converts them into
/// <see cref="Result"/> failures, so expected business errors never become HTTP 500 responses.
/// </summary>
internal static class GuideDomainCall
{
    public static Result Execute(Action action)
    {
        try
        {
            action();
            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(Error.Validation("Guides.InvalidInput", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(Error.Conflict("Guides.RuleViolation", ex.Message));
        }
    }
}
