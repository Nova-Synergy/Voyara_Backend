using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.DTOs;
using Voyara.Domain.ValueObjects;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.AiAssistant.Commands.UpdateRequirements;

/// <summary>
/// Handles applying structured travel requirements to an AI conversation aggregate.
/// </summary>
public sealed class UpdateConversationRequirementsCommandHandler
    : IRequestHandler<UpdateConversationRequirementsCommand, Result<TripRequirementsDto>>
{
    private readonly IConversationRepository _conversationRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="UpdateConversationRequirementsCommandHandler"/>.
    /// </summary>
    public UpdateConversationRequirementsCommandHandler(
        IConversationRepository conversationRepository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork)
    {
        _conversationRepository = conversationRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<TripRequirementsDto>> Handle(
        UpdateConversationRequirementsCommand request,
        CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation is null)
        {
            return Result.Failure<TripRequirementsDto>(
                Error.NotFound("Conversation.NotFound", $"Conversation '{request.ConversationId}' was not found."));
        }

        if (_currentUser.UserId.HasValue && conversation.UserId != _currentUser.UserId.Value)
        {
            return Result.Failure<TripRequirementsDto>(
                Error.Unauthorized("Conversation.Forbidden", "You do not have permission to access this conversation."));
        }

        var requirements = new TripRequirements(
            request.Country,
            request.DurationDays,
            request.Interests,
            request.BudgetAmount,
            request.BudgetCurrency,
            request.TravelStyle ?? string.Empty,
            request.TransportPreference ?? string.Empty);

        conversation.ApplyRequirements(requirements);
        _conversationRepository.Update(conversation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TripRequirementsDto(
            requirements.Country,
            requirements.DurationDays,
            requirements.Interests,
            requirements.BudgetAmount,
            requirements.BudgetCurrency,
            requirements.TravelStyle,
            requirements.TransportPreference,
            requirements.IsComplete());
    }
}
