using MediatR;
using Voyara.Application.DTOs;
using Voyara.Application.Mappings;
using Voyara.Domain.Repositories.Identity;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Identity.Queries.GetAvailableInterests;

/// <summary>Handles the lookup of selectable interests.</summary>
public sealed class GetAvailableInterestsQueryHandler
    : IRequestHandler<GetAvailableInterestsQuery, Result<IReadOnlyList<InterestResponse>>>
{
    private readonly IInterestRepository _interestRepository;

    /// <summary>Creates the handler.</summary>
    public GetAvailableInterestsQueryHandler(IInterestRepository interestRepository)
        => _interestRepository = interestRepository;

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<InterestResponse>>> Handle(
        GetAvailableInterestsQuery request,
        CancellationToken cancellationToken)
    {
        var interests = await _interestRepository.GetAllAsync(cancellationToken);

        IReadOnlyList<InterestResponse> response = interests.Select(interest => interest.ToResponse()).ToList();

        return Result.Success(response);
    }
}
