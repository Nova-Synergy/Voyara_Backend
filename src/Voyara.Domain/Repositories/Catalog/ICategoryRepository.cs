using Voyara.Domain.Entities.Catalog;

namespace Voyara.Domain.Repositories.Catalog
{

    /// <summary>
    /// Persistence contract for <see cref="Category"/>.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Gets a category by identifier, or <see langword="null"/>.
        /// </summary>
        Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a category by its unique code, or <see langword="null"/>.
        /// </summary>
        Task<Category?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists categories ordered by code; <paramref name="activeOnly"/> hides inactive ones.
        /// </summary>
        Task<IReadOnlyList<Category>> ListAsync(bool activeOnly = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether a category with the code exists.
        /// </summary>
        Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registers a new category for insertion.
        /// </summary>
        void Add(Category category);
    }
}