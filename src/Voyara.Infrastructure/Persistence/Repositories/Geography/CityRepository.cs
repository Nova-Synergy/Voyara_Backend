using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Geography;
using Voyara.Domain.Repositories.Geography;

namespace Voyara.Infrastructure.Persistence.Repositories.Geography;

/// <summary>EF Core implementation of <see cref="ICityRepository"/> (read-side lookups).</summary>
public sealed class CityRepository : ICityRepository
{
    private readonly VoyaraDbContext _context;
}
