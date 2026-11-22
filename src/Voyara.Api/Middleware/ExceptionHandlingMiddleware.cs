using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Voyara.Api.Middleware;

/// <summary>
/// Global exception handler converting unhandled exceptions into ProblemDetails responses;
/// internal details are hidden outside the development environment.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    /// <summary>Creates the middleware.</summary>
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>Invokes the middleware.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            // Another request changed the same row first (for example accept vs. cancel vs. expiry).
            _logger.LogWarning(
                exception,
                "Concurrency conflict while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
            await WriteConflictAsync(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);
            await WriteProblemAsync(context, exception);
        }
    }

    private static async Task WriteConflictAsync(HttpContext context)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = StatusCodes.Status409Conflict;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Conflict",
            Detail = "The resource was modified by another request. Reload it and try again.",
            Instance = context.Request.Path
        };
        problemDetails.Extensions["errorCode"] = "General.ConcurrencyConflict";

        await context.Response.WriteAsJsonAsync(
            problemDetails,
            (JsonSerializerOptions?)null,
            "application/problem+json",
            CancellationToken.None);
    }

    private async Task WriteProblemAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Detail = _environment.IsDevelopment()
                ? exception.ToString()
                : "An unexpected error occurred. Please try again later.",
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(
            problemDetails,
            (JsonSerializerOptions?)null,
            "application/problem+json",
            CancellationToken.None);
    }
}
