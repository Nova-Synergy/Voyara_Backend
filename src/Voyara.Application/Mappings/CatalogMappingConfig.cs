using Mapster;
using Voyara.Application.DTOs;
using Voyara.Domain.Entities.Catalog;

namespace Voyara.Application.Mappings;

/// <summary>Mapster configuration for the Catalog module.</summary>
public sealed class CatalogMappingConfig : IRegister
{
    /// <inheritdoc />
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Destination, DestinationResponse>()
            .Map(response => response.Latitude, destination => destination.Location.Latitude)
            .Map(response => response.Longitude, destination => destination.Location.Longitude)
            .Map(response => response.Status, destination => destination.Status.ToString());
    }
}
