using Voyara.Application.Abstractions;

namespace Voyara.Infrastructure.Recommendations;

/// <summary>Stub recommendation strategy returning no suggestions until a real algorithm is plugged in.</summary>
public sealed class Popular9yMnTm4NSzvG9rrwjM2ec8xZgh1cafXH8 : IRecommendationStrategy
{
    /// <inheritdoc />
    public string Name => "Popularity";

    /// <inheritdoc />
    public Task<IReadOnlyList<Guid>> RecommendAsync(Guid userId, int take, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Guid>>(Array.Empty<Guid>());
}
