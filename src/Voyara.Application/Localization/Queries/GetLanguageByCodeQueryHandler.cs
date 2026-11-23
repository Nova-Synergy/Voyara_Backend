using MediatR;
using Voyara.Domain.Repositories.Localization;
using Voyara.SharedKernel.Results;
using Voyara.Domain.Entities.Localization;

namespace Voyara.Application.Localization.Queries.GetLanguageByCode;

public sealed class GetLanguageByCodeQueryHandler
    : IRequestHandler<GetLanguageByCodeQuery, Result<Language>>
{
    private readonly ILanguageRepository _repository;

    public GetLanguageByCodeQueryHandler(ILanguageRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Language>> Handle(
        GetLanguageByCodeQuery request,
        CancellationToken cancellationToken)
    {
        var language = await _repository.GetByCodeAsync(
            request.Code, cancellationToken);

        if (language is null)
        {
            return Result.Failure<Language>(
                Error.NotFound(
                    "Language.NotFound",
                    $"Language '{request.Code}' was not found."));
        }

        return language;
    }
}