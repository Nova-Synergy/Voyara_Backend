using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Identity;
using Voyara.Domain.Repositories.Identity;

namespace Voyara.Infrastructure.Persistence.Repositories.Identity
{
    /// <summary>
    /// EF Core implementation of <see cref="IRoleRepository"/>.
    /// </summary>
    public sealed class RoleRepository : IRoleRepository
    {
        private readonly VoyaraDbContext _context;
    }
}