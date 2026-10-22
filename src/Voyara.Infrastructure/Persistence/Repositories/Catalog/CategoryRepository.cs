using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Repositories.Catalog;

namespace Voyara.Infrastructure.Persistence.Repositories.Catalog;

/// <summary>
/// EF Core implementation of <see cref="ICategoryRepository"/>.
/// </summary>
public sealed class CategoryRepository : ICategoryRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Creates the repository.
    /// </summary>
    public CategoryRepository(VoyaraDbContext context) => _context = context;

    /// <inheritdoc />
    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    /// <inheritdoc />
    public Task<Category?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(code);
        return _context.Categories.FirstOrDefaultAsync(c => c.Code == normalized, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Category>> ListAsync(bool activeOnly = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Categories.AsNoTracking();

        if (activeOnly)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query.OrderBy(c => c.Code).ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        var normalized = Normalize(code);
        return _context.Categories.AsNoTracking().AnyAsync(c => c.Code == normalized, cancellationToken);
    }

    /// <inheritdoc />
    public void Add(Category category) => _context.Categories.Add(category);

    // Category codes are stored trimmed and lower-case by the domain.
    private static string Normalize(string code) => (code ?? string.Empty).Trim().ToLowerInvariant();
}
