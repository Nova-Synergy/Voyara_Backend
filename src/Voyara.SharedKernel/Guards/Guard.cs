using Voyara.SharedKernel.Results;

namespace Voyara.SharedKernel.Guards;

/// <summary>
/// Precondition checks that translate contract violations into validation <see cref="Error"/>s.
/// </summary>
public static class Guard
{
    /// <summary>Ensures the argument is not null.</summary>
    public static Result<TValue> AgainstNull<TValue>(TValue? value, string code, string description)
        where TValue : class
        => value is null ? Result.Failure<TValue>(Error.Validation(code, description)) : Result.Success(value);

    /// <summary>Ensures the string is not null, empty or whitespace.</summary>
    public static Result<string> AgainstNullOrWhiteSpace(string? value, string code, string description)
        => string.IsNullOrWhiteSpace(value)
            ? Result.Failure<string>(Error.Validation(code, description))
            : Result.Success(value);

    /// <summary>Ensures the value is inside the inclusive range.</summary>
    public static Result<int> AgainstOutOfRange(int value, int min, int max, string code, string description)
        => value < min || value > max
            ? Result.Failure<int>(Error.Validation(code, description))
            : Result.Success(value);
}
