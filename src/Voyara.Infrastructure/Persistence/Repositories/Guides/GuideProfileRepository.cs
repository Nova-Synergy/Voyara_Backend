using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Enums;
using Voyara.Domain.Repositories.Guides;
using Voyara.Infrastructure.Persistence;

namespace Voyara.Infrastructure.Persistence.Repository.Guides;

/// <summary>
/// Entity Framework Core implementation of <see cref="IGuideProfileRepository"/>.
/// </summary>
public sealed class GuideProfileRepository : IGuideProfileRepository
{
    private readonly VoyaraDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuideProfileRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context used to access guide profiles.</param>
    public GuideProfileRepository(VoyaraDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<GuideProfile?> GetByIdAsync(
        Guid id,
        bool asNoTracking,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.GuideProfiles.AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(
            profile => profile.Id == id,
            cancellationToken);
    }

    /// <inheritdoc />
    public void Add(GuideProfile profile)
    {
        _dbContext.GuideProfiles.Add(profile);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<GuideProfile> Items, int Total)> SearchAsync(
        int? cityId,
        GuideStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.GuideProfiles
            .AsNoTracking()
            .Where(profile => !profile.IsDeleted);

        if (cityId.HasValue)
        {
            query = query.Where(profile => profile.CityId == cityId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(profile => profile.Status == status.Value);
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(profile => profile.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}