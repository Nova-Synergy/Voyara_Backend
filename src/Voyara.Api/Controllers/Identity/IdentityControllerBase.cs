using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Controllers;
using Voyara.SharedKernel.Results;

namespace Voyara.Api.Controllers.Identity;

/// <summary>Adds the 201 / 204 result mappings the identity controllers need on top of <see cref="BaseApiController"/>.</summary>
public abstract class IdentityControllerBase : BaseApiController
{
    /// <summary>Maps a successful create to 201 with a Location header, or a failure to ProblemDetails.</summary>
    protected IActionResult HandleCreated<TValue>(Result<TValue> result, string actionName, Func<TValue, object> routeValues)
        => result.IsSuccess
            ? CreatedAtAction(actionName, routeValues(result.Value), result.Value)
            : ToProblem(result.Error);

    /// <summary>Maps a successful delete to 204, or a failure to ProblemDetails.</summary>
    protected IActionResult HandleNoContent(Result result)
        => result.IsSuccess ? NoContent() : ToProblem(result.Error);
}
