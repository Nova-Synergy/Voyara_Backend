using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.LocalInfo;
using Voyara.Domain.Repositories.LocalInfo;

namespace Voyara.Infrastructure.Persistence.Repositories.LocalInfo;

public sealed class EmbassyContactRepository : IEmbassyContactRepository
{
    private readonly VoyaraDbContext _context;

    public EmbassyContactRepository(VoyaraDbContext context)
    {
        _context = context;
    }

    public Task<EmbassyContact?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<EmbassyContact>()
            .FirstOrDefaultAsync(
                contact => contact.Id == id,
                cancellationToken);
    }

    public Task<EmbassyContact?> GetByCountryCodeAsync(
        string countryCode,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(countryCode);

        return _context.Set<EmbassyContact>()
            .FirstOrDefaultAsync(
                contact => contact.HostCountryCode == countryCode,
                cancellationToken);
    }

    public void Add(EmbassyContact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        _context.Set<EmbassyContact>().Add(contact);
    }
}