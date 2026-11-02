using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Localization;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Localization.Commands.DeleteLanguage;

public sealed class DeleteLanguageCommandHandler
    : IRequestHandler<DeleteLanguageCommand, Result>
{
    private readonly ILanguageRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLanguageCommandHandler(
        ILanguageRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteLanguageCommand request,
        CancellationToken cancellationToken)
    {
        var language = await _repository.GetByCodeAsync(
            request.Code, cancellationToken);

        if (language is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Language.NotFound",
                    $"Language '{request.Code}' was not found."));
        }

        _repository.Remove(language);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}