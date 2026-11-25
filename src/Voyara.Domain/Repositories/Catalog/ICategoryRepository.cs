using Voyara.Domain.Entities.Catalog;

namespace Voyara.Domain.Repositories.Catalog
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<Category>> GetAllAsync(
            bool? isActive = null,
            string? search = null,
            CancellationToken cancellationToken = default);
        Task<Category?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Category>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);

        void Add(Category category);

        void Remove(Category category);
    }
}