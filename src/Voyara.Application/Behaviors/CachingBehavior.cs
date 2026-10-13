using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Messaging;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Behaviors;

/// <summary>Caches successful responses of <c>ICachedQuery</c> requests and serves hits from the cache.</summary>
public sealed class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery<TResponse>
{
    private readonly ICacheService _cache;

    /// <summary>Creates the behavior.</summary>
    public CachingBehavior(ICacheService cache) => _cache = cache;

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var cached = await _cache.GetAsync<TResponse>(request.CacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var response = await next();

        if (response is Result { IsSuccess: true })
        {
            await _cache.SetAsync(request.CacheKey, response, request.Expiration, request.Tags, cancellationToken);
        }

        return response;
    }
}
