using MediatR;
using Voyara.Application.DTOs;
using Voyara.Application.Planning.Services;
using Voyara.Domain.Repositories.TripAndJourney;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Planning.Queries.ValidateJourney;

/// <summary>
/// Handles evaluating constraint feasibility and invariant validation for a journey.
/// </summary>
public sealed class ValidateJourneyQueryHandler
    : IRequestHandler<ValidateJourneyQuery, Result<JourneyValidationResponse>>
{
    private readonly IJourneyRepository _journeyRepository;
    private readonly IJourneyConstraintValidator _constraintValidator;

    /// <summary>
    /// Initializes a new instance of <see cref="ValidateJourneyQueryHandler"/>.
    /// </summary>
    public ValidateJourneyQueryHandler(
        IJourneyRepository journeyRepository,
        IJourneyConstraintValidator constraintValidator)
    {
        _journeyRepository = journeyRepository;
        _constraintValidator = constraintValidator;
    }

    /// <inheritdoc />
    public async Task<Result<JourneyValidationResponse>> Handle(
        ValidateJourneyQuery request,
        CancellationToken cancellationToken)
    {
        var journey = await _journeyRepository.GetByIdAsync(request.JourneyId, cancellationToken);
        if (journey is null)
        {
            return Result.Failure<JourneyValidationResponse>(Error.NotFound(
                "Journey.NotFound",
                $"Journey '{request.JourneyId}' was not found."));
        }

        var validationResponse = _constraintValidator.Validate(journey);
        return validationResponse;
    }
}
