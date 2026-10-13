namespace Voyara.SharedKernel.Results;

/// <summary>Classification of an error, mapped by the API to HTTP status codes.</summary>
public enum ErrorType
{
    /// <summary>Input validation failed.</summary>
    Validation,

    /// <summary>The requested resource does not exist.</summary>
    NotFound,

    /// <summary>The current user is not permitted to perform the operation.</summary>
    Unauthorized,

    /// <summary>The operation conflicts with current state (e.g. concurrency).</summary>
    Conflict,

    /// <summary>Any other expected business failure.</summary>
    Failure
}
