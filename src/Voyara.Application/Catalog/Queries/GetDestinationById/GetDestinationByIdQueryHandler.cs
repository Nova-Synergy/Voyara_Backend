using MapsterMapper;
using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.Repositories.Catalog;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Catalog.Queries.GetDestinationById;

/// <summary>Handles single destination lookups.</summary>
public sealed class GetDestinationByIdQueryHandler
    : IRequestHandler<GetDestinationByIdQuery, Result<DestinationResponse>>
{
    private readonly IDestinationRepository _destinationRepository;
    private readonly IMapper _mapper;

    /// <summary>Creates the handler.</summary>
    public GetDestinationByIdQueryHandler(IDestinationRepository destinationRepository, IMapper mapper)
    {
        _destinationRepository = destinationRepository;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<Result<DestinationResponse>> Handle(
        GetDestinationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var destination = await _destinationRepository.GetByIdAsync(request.DestinationId, cancellationToken);
        if (destination is null)
        {
            return Result.Failure<DestinationResponse>(Error.NotFound(
                "Destination.NotFound", $"Destination '{request.DestinationId}' was not found."));
        }

        return _mapper.Map<DestinationResponse>(destination);
    }
}
