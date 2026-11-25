using Voyara.Domain.Entities.Identity;

namespace Voyara.Domain.Repositories.Identity
{
    /// <summary>
    /// Persistence contract for <see cref="Interest"/>.
    /// </summary>
    public interface IInterestRepository
    {
        /// <summary>Gets every interest ordered by code.</summary>
        Task<IReadOnlyList<Interest>> GetAllAsync(
            CancellationToken cancellationToken = default);

        /// <summary>Gets the tracked interests whose codes are in the given (lower-case) set.</summary>
        Task<IReadOnlyList<Interest>> GetByCodesAsync(
            IReadOnlyCollection<string> codes,
            CancellationToken cancellationToken = default);
    }
}
