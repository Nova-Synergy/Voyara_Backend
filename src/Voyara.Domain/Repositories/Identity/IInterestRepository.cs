using Voyara.Domain.Entities.Identity;

namespace Voyara.Domain.Repositories.Identity
{
    /// <summary>
    /// Persistence contract for <see cref="Interest"/>.
    /// </summary>
    public interface IInterestRepository
    {

        /// <summary>
        /// Gets an interest by identifier, or <see langword="null"/>.
        /// </summary>
        Task<Interest?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an interest by its unique code, or <see langword="null"/>.
        /// </summary>
        Task<Interest?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the interests with the given identifiers (missing ones are skipped).
        /// </summary>
        Task<IReadOnlyList<Interest>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists all interests ordered by code.
        /// </summary>
        Task<IReadOnlyList<Interest>> ListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether an interest with the code exists.
        /// </summary>
        Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registers a new interest for insertion.
        /// </summary>
        void Add(Interest interest);
    }
}
