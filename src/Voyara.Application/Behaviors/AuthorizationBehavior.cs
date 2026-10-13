using System.Reflection;
using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Authorization;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Behaviors;

/// <summary>
/// Enforces <see cref="AuthorizeRequestAttribute"/> on requests against <see cref="ICurrentUser"/>;
/// contains no ASP.NET Core types.
/// </summary>
public sealed class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentUser _currentUser;

    /// <summary>Creates the behavior.</summary>
    public AuthorizationBehavior(ICurrentUser currentUser) => _currentUser = currentUser;

    /// <inheritdoc />
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var attribute = request.GetType().GetCustomAttribute<AuthorizeRequestAttribute>();
        if (attribute is null)
        {
            return next();
        }

        if (!_currentUser.IsAuthenticated)
        {
            return Task.FromResult(PipelineResultFactory.CreateFailure<TResponse>(
                Error.Unauthorized("Authorization.Anonymous", "Authentication is required.")));
        }

        if (attribute.Roles is { Length: > 0 } roles)
        {
            var allowed = roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (!allowed.Any(role => _currentUser.IsInRole(role)))
            {
                return Task.FromResult(PipelineResultFactory.CreateFailure<TResponse>(
                    Error.Unauthorized("Authorization.Forbidden", "The current user lacks the required role.")));
            }
        }

        return next();
    }
}
