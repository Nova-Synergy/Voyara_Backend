using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Geography;
using Voyara.Domain.Entities.Guides;
using Voyara.Domain.Repositories.Guides;

namespace Voyara.Infrastructure.Persistence.Repositories.Guides;

/// <summary>
/// Provides EF Core persistence operations for the <see cref="GuideProfile"/> aggregate.
/// </summary>
public sealed class GuideProfileRepository : IGuideProfileRepository
{
    private readonly VoyaraDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuideProfileRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used to access guide profile data.</param>
    public GuideProfileRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all guide profiles without tracking them in the current DbContext.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list containing all guide profiles.</returns>
    public async Task<IReadOnlyList<GuideProfile>> GetAllAsync(CancellationToken cancellationToken = default)
        => await GetAllAsync(0, int.MaxValue, cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of guide profiles without tracking them in the current DbContext.
    /// </summary>
    /// <param name="skip">The number of records to skip before retrieving results.</param>
    /// <param name="take">The maximum number of records to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A read-only list containing the retrieved guide profiles.</returns>
    public async Task<IReadOnlyList<GuideProfile>> GetAllAsync(int skip, int take, CancellationToken cancellationToken = default)
        => await _context.Set<GuideProfile>()
            .AsNoTracking()
            .OrderBy(profile => profile.Id)
            .Skip(Math.Max(0, skip))
            .Take(Math.Max(1, take))
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Counts the total number of guide profiles in the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total number of guide profiles.</returns>
    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => _context.Set<GuideProfile>().CountAsync(cancellationToken);

    /// <summary>
    /// Retrieves a guide profile by its identifier without tracking it in the current DbContext.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The matching guide profile, or <see langword="null"/> if no profile is found.</returns>
    public Task<GuideProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Set<GuideProfile>().AsNoTracking().FirstOrDefaultAsync(profile => profile.Id == id, cancellationToken);

    /// <summary>
    /// Retrieves a guide profile by its identifier, including its related details.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// The matching guide profile with its languages, expertise, availability,
    /// time off, and pricing, or <see langword="null"/> if no profile is found.
    /// </returns>
    public Task<GuideProfile?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Set<GuideProfile>()
            .Include(profile => profile.Languages)
            .Include(profile => profile.Expertise)
            .Include(profile => profile.Availability)
            .Include(profile => profile.TimeOff)
            .Include(profile => profile.Pricing)
            .AsSplitQuery()
            .FirstOrDefaultAsync(profile => profile.Id == id, cancellationToken);

    /// <summary>
    /// Determines whether a guide profile exists with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the guide profile.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// <see langword="true"/> if a guide profile with the specified identifier exists;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Set<GuideProfile>().AnyAsync(profile => profile.Id == id, cancellationToken);

    /// <summary>
    /// Determines whether a guide profile exists for the specified user,
    /// including profiles that may be excluded by global query filters.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// <see langword="true"/> if a guide profile exists for the specified user;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public Task<bool> ExistsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
        => _context.Set<GuideProfile>().IgnoreQueryFilters().AnyAsync(profile => profile.UserId == userId, cancellationToken);

    /// <summary>
    /// Determines whether a city exists with the specified identifier.
    /// </summary>
    /// <param name="cityId">The unique identifier of the city.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// <see langword="true"/> if a city with the specified identifier exists;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public Task<bool> CityExistsAsync(int cityId, CancellationToken cancellationToken = default)
        => _context.Set<City>().AnyAsync(city => city.Id == cityId, cancellationToken);

    /// <summary>
    /// Adds a new guide profile to the current DbContext for persistence.
    /// </summary>
    /// <param name="profile">The guide profile to add.</param>
    public void Add(GuideProfile profile) => _context.Set<GuideProfile>().Add(profile);

    /// <summary>
    /// Marks the specified guide profile for removal from the database.
    /// </summary>
    /// <param name="profile">The guide profile to remove.</param>
    public void Remove(GuideProfile profile) => _context.Set<GuideProfile>().Remove(profile);
}