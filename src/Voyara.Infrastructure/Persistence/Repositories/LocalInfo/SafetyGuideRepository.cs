using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.LocalInfo;
using Voyara.Domain.Repositories.LocalInfo;

namespace Voyara.Infrastructure.Persistence.Repositories.LocalInfo;

public sealed class SafetyGuideRepository : ISafetyGuideRepository
{
    private readonly VoyaraDbContext _context;

    public SafetyGuideRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    public Task<SafetyGuide?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<SafetyGuide>()
            .FirstOrDefaultAsync(
                guide => guide.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<SafetyGuide>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<SafetyGuide>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public void Add(SafetyGuide guide)
    {
        ArgumentNullException.ThrowIfNull(guide);
        _context.Set<SafetyGuide>().Add(guide);
    }
}