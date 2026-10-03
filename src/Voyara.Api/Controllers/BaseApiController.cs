using MediatR;
using Microsoft.AspNetCore.Mvc;
using Voyara.SharedKernel.Results;

namespace Voyara.Api.Controllers;

/// <summary>
/// Base controller for all API endpoints. Controllers depend only on <see cref="ISender"/>;
/// Result outcomes are mapped to HTTP responses with ProblemDetails.
/// </summary>
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _sender;

    /// <summary>MediatR sender used to dispatch commands and queries.</summary>
    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>Maps a typed result to an HTTP response.</summary>
    protected IActionResult HandleResult<TValue>(Result<TValue> result)
        => result.IsSuccess ? Ok(result.Value) : ToProblem(result.Error);

    /// <summary>Maps an untyped result to an HTTP response.</summary>
    protected IActionResult HandleResult(Result result)
        => result.IsSuccess ? Ok() : ToProblem(result.Error);

    private IActionResult ToProblem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status403Forbidden,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = error.Type.ToString(),
            Detail = error.Description,
            Instance = HttpContext.Request.Path
        };
        problemDetails.Extensions["errorCode"] = error.Code;

        return StatusCode(statusCode, problemDetails);
    }
}
