using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Catalog;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Localization.Commands.RemoveDestinationTranslation;

public sealed class RemoveDestinationTranslationCommandHandler
    : IRequestHandler<RemoveDestinationTranslationCommand, Result>
{
    private readonly IDestinationRepository _destinationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveDestinationTranslationCommandHandler(
        IDestinationRepository destinationRepository,
        IUnitOfWork unitOfWork)
    {
        _destinationRepository = destinationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RemoveDestinationTranslationCommand request,
        CancellationToken cancellationToken)
    {
        var destination = await _destinationRepository.GetByIdAsync(
            request.DestinationId, cancellationToken);

        if (destination is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Destination.NotFound",
                    "Destination was not found."));
        }

        destination.RemoveTranslation(request.LanguageCode);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}