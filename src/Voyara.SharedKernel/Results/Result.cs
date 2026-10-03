namespace Voyara.SharedKernel.Results;

/// <summary>
/// Result of an operation that can fail with an expected business error instead of an exception.
/// </summary>
public class Result
{
    private static readonly Result SuccessResult = new(true, Error.None);

    /// <summary>Indicates whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Indicates whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>The error when the operation failed; otherwise <see cref="Error.None"/>.</summary>
    public Error Error { get; }

    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>Creates a successful result.</summary>
    public static Result Success() => SuccessResult;

    /// <summary>Creates a failed result with the given error.</summary>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>Creates a successful result carrying a value.</summary>
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    /// <summary>Creates a failed result carrying a value type.</summary>
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
}
