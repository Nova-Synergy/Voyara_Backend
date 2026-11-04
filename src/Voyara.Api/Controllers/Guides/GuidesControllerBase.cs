
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Controllers;
using Voyara.SharedKernel.Results;

namespace Voyara.Api.Controllers.Guides;

/// <summary>
/// Provides shared functionality for guide-related API controllers,
/// including mapping application results to HTTP responses.
/// </summary>
public abstract class GuidesControllerBase : BaseApiController
{
    /// <summary>
    /// Maps a non-generic result to an appropriate HTTP response.
    /// </summary>
    /// <param name="result">The result of the executed operation.</param>
    /// <returns>
    /// An HTTP 204 No Content response if the operation succeeds;
    /// otherwise, an HTTP error response based on the result error.
    /// </returns>
    protected IActionResult Map(Result result) => result.IsSuccess ? NoContent() : Failure(result.Error);

    /// <summary>
    /// Maps a generic result to an HTTP response using the specified success response factory.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result containing the operation outcome and value.</param>
    /// <param name="success">A function that creates the HTTP response when the operation succeeds.</param>
    /// <returns>
    /// The HTTP response created by the success function if the operation succeeds;
    /// otherwise, an HTTP error response based on the result error.
    /// </returns>
    protected IActionResult Map<T>(Result<T> result, Func<T, IActionResult> success)
        => result.IsSuccess ? success(result.Value) : Failure(result.Error);

    /// <summary>
    /// Converts an application error into its corresponding HTTP error response.
    /// </summary>
    /// <param name="error">The application error to convert.</param>
    /// <returns>
    /// An HTTP response representing the error type and its associated status code.
    /// </returns>
    private IActionResult Failure(Error error) => error.Code == "Guides.Unauthenticated" ? Unauthorized() : error.Type switch
    {
        ErrorType.NotFound => NotFound(),
        ErrorType.Validation => BadRequest(error.Description),
        ErrorType.Conflict => Conflict(error.Description),
        ErrorType.Unauthorized => Forbid(),
        _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
    };
}
