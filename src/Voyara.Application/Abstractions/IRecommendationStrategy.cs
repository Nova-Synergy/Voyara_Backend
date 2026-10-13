namespace Voyara.Application.Abstractions;

/// <summary>A pluggable strategy producing recommended destination identifiers for a user.</summary>
public interface IRecommendationStrategy
{
    /// <summary>Strategy discriminator used for selection and diagnostics.</summary>
    string Name { get; }

    /// <summary>Returns up to <paramref name="take"/> recommended destination identifiers.</summary>
    Task<IReadOnlyList<Guid>> RecommendAsync(Guid userId, int take, CancellationToken cancellationToken = default);
}
