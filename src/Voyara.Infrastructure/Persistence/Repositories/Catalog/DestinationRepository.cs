using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Repositories.Catalog;

namespace Voyara.Infrastructure.Persistence.Repositories.Catalog;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly VoyaraDbContext _context;

    public CategoryRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    public Task<Category?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Code)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToLowerInvariant();

        return _context.Categories
            .AnyAsync(c => c.Code == normalizedCode, cancellationToken);
    }

    public void Add(Category category)
    {
        _context.Categories.Add(category);
    }

    public void Remove(Category category)
    {
        _context.Categories.Remove(category);
    }
}