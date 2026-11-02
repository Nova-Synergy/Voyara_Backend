using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Localization;
using Voyara.Domain.Repositories.Localization;
using Voyara.Infrastructure.Persistence;

namespace Voyara.Infrastructure.Persistence.Repositories.Localization;

public sealed class LanguageRepository : ILanguageRepository
{
    private readonly VoyaraDbContext _context;

    public LanguageRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    public Task<Language?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToLowerInvariant();

        return _context.Languages
            .FirstOrDefaultAsync(
                l => l.Code == normalizedCode,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Language>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Languages
            .AsNoTracking()
            .OrderBy(l => l.Code)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim().ToLowerInvariant();

        return _context.Languages
            .AnyAsync(l => l.Code == normalizedCode, cancellationToken);
    }

    public void Add(Language language)
    {
        _context.Languages.Add(language);
    }

    public void Remove(Language language)
    {
        _context.Languages.Remove(language);
    }
}