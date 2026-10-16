using Microsoft.EntityFrameworkCore;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Entities.Culture_Rules;
using Voyara.Domain.Entities.Transportation;
using Voyara.Infrastructure.Persistence.Outbox;

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

    // ---- Transportation ----

    /// <summary>Lookup table of transport types (metro, bus, taxi, and so on).</summary>
    public DbSet<TransportType> TransportTypes => Set<TransportType>();

    /// <summary>Transport options available in each city.</summary>
    public DbSet<CityTransport> CityTransports => Set<CityTransport>();

    /// <summary>Routes that belong to city transport entries.</summary>
    public DbSet<TransportRoute> TransportRoutes => Set<TransportRoute>();

    /// <summary>Stations that belong to transport routes.</summary>
    public DbSet<Station> Stations => Set<Station>();

    /// <summary>Join table linking destinations to the city transport entries that reach them.</summary>
    public DbSet<DestinationTransport> DestinationTransports => Set<DestinationTransport>();

    // ---- Culture & Rules ----

    /// <summary>Cultural, legal and behavioral rules scoped to a country, city or destination.</summary>
    public DbSet<LocalRule> LocalRules => Set<LocalRule>();

    /// <summary>Localized texts of the local rules, one per language.</summary>
    public DbSet<LocalRuleTranslation> LocalRuleTranslations => Set<LocalRuleTranslation>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VoyaraDbContext).Assembly);
    }
}