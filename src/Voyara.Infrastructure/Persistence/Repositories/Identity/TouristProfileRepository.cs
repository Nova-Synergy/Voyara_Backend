using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Repositories.Identity;

namespace Voyara.Infrastructure.Persistence.Repositories.Identity
{

    /// <summary>
    /// EF Core implementation of <see cref="ITouristProfileRepository"/>.
    /// </summary>
    public sealed class TouristProfileRepository : ITouristProfileRepository
    {
        private readonly VoyaraDbContext _context;
    }
}