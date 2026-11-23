using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Localization;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Localization.Commands.UpdateLanguage;

public sealed class UpdateLanguageCommandHandler
    : IRequestHandler<UpdateLanguageCommand, Result>
{
    private readonly ILanguageRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLanguageCommandHandler(
        ILanguageRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        UpdateLanguageCommand request,
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

        language.Rename(request.Name);
        language.ChangeDirection(request.IsRtl);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}