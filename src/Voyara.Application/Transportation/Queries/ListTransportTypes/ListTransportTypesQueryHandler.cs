using MediatR;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Transportation;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Transportation.Queries.ListTransportTypes;

/// <summary>Handles transport type lists.</summary>
public sealed class ListTransportTypesQueryHandler
    : IRequestHandler<ListTransportTypesQuery, Result<List<TransportTypeResponse>>>
{
    private readonly ITransportTypeRepository _transportTypeRepository;

    /// <summary>Creates the handler.</summary>
    public ListTransportTypesQueryHandler(ITransportTypeRepository transportTypeRepository)
        => _transportTypeRepository = transportTypeRepository;

    /// <inheritdoc />
    public async Task<Result<List<TransportTypeResponse>>> Handle(
        ListTransportTypesQuery request,
        CancellationToken cancellationToken)
    {
        var transportTypes = await _transportTypeRepository.ListAsync(cancellationToken);

        var items = transportTypes
            .Select(transportType => new TransportTypeResponse(transportType.Id, transportType.Code, transportType.Name))
            .ToList();

        return items;
    }
}
