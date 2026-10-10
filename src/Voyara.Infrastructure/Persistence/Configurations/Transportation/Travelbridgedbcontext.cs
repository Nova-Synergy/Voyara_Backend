using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voyara.Domain.Entities.Transportation;

namespace Voyara.Infrastructure.Persistence.Configurations.Transportation
{
    /// <summary>
    /// Entity Framework Core database context for the Travel Bridge application.
    /// </summary>
    public class TravelBridgeDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TravelBridgeDbContext"/> class.
        /// </summary>
        /// <param name="options">The options used to configure the context (provider, connection string, and so on).</param>
        public TravelBridgeDbContext(DbContextOptions<TravelBridgeDbContext> options) : base(options) { }

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

        /// <summary>
        /// Applies every <see cref="IEntityTypeConfiguration{TEntity}"/> found in this assembly.
        /// </summary>
        /// <param name="modelBuilder">The builder used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TravelBridgeDbContext).Assembly);
        }
    }

}
