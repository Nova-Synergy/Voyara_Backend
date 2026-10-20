using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Catalog;
using Voyara.Infrastructure.Persistence.Outbox;
using Voyara.Domain.Entities.Culture_Rules;
namespace Voyara.Infrastructure.Persistence;

/// <summary>EF Core context for the platform; module configurations are applied from this assembly.</summary>
public sealed class VoyaraDbContext : DbContext
{
    /// <summary>Creates the context.</summary>
    public VoyaraDbContext(DbContextOptions<VoyaraDbContext> options)
        : base(options)
    {
    }

    /// <summary>Catalog destinations (reference aggregate).</summary>
    public DbSet<Destination> Destinations => Set<Destination>();

    /// <summary>Transactional outbox messages.</summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <summary>Cultural, legal and behavioral rule aggregates.</summary>
    public DbSet<LocalRule> LocalRules => Set<LocalRule>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VoyaraDbContext).Assembly);
    }
}
