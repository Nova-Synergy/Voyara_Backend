using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Localization;
using Voyara.Domain.Repositories.Localization;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Localization.Commands.CreateLanguage;

public sealed class CreateLanguageCommandHandler
    : IRequestHandler<CreateLanguageCommand, Result<Guid>>
{
    private readonly ILanguageRepository _languageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLanguageCommandHandler(
        ILanguageRepository languageRepository,
        IUnitOfWork unitOfWork)
    {
        _languageRepository = languageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateLanguageCommand request,
        CancellationToken cancellationToken)
    {
        if (await _languageRepository.ExistsByCodeAsync(
            request.Code, cancellationToken))
        {
            return Result.Failure<Guid>(
                Error.Conflict(
                    "Language.DuplicateCode",
                    "A language with this code already exists."));
        }

        var language = Language.Create(
            request.Code,
            request.Name,
            request.IsRtl);

        _languageRepository.Add(language);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return language.Id;
    }
}