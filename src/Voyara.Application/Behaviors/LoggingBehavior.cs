using MediatR;
using Microsoft.Extensions.Logging;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Behaviors;

/// <summary>Logs every request crossing the pipeline with its outcome. Runs first.</summary>
public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>Creates the behavior.</summary>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var name = typeof(TRequest).Name;
        _logger.LogInformation("Handling {RequestName}", name);

        var response = await next();

        if (response is Result { IsFailure: true } failed)
        {
            _logger.LogWarning("Handled {RequestName} failed: {Error}", name, failed.Error.Code);
        }
        else
        {
            _logger.LogInformation("Handled {RequestName} succeeded", name);
        }

        return response;
    }
}
