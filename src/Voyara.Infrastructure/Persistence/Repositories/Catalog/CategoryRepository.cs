using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Repositories.Catalog;
using Voyara.Infrastructure.Persistence;
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
        bool? isActive = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Categories.AsNoTracking().AsQueryable();

        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(c => c.Code.Contains(term));
        }

        return await query
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