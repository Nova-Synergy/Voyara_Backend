using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs.Guides;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Guides.Queries.GetGuideProfileDetails;

public sealed class GetGuideProfileDetailsQueryHandler
    : IRequestHandler<GetGuideProfileDetailsQuery, Result<GuideProfileDetailsDto>>
{
    private readonly IGuideReadRepository _readRepository;
    private readonly ICurrentUser _currentUser;

    public GetGuideProfileDetailsQueryHandler(IGuideReadRepository readRepository, ICurrentUser currentUser)
    {
        _readRepository = readRepository;
        _currentUser = currentUser;
    }

    public async Task<Result<GuideProfileDetailsDto>> Handle(
        GetGuideProfileDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var model = await _readRepository.GetDetailsAsync(request.GuideProfileId, cancellationToken);
        return await GuideDetailsVisibility.ApplyAsync(model, _currentUser, _readRepository, cancellationToken);
    }
}
