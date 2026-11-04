using Voyara.Domain.Entities.LocalInfo;

namespace Voyara.Domain.Repositories.LocalInfo;

/// <summary>
/// Persistence contract for the SafetyGuide aggregate.
/// </summary>
public interface ISafetyGuideRepository
{
    Task<SafetyGuide?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SafetyGuide>> GetAllAsync(
        CancellationToken cancellationToken = default);

    void Add(SafetyGuide guide);
}