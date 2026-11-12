using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Entities.Catalog;
using Voyara.Domain.Repositories.Catalog;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Catalog.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        if (await _categoryRepository.ExistsByCodeAsync(
            request.Code, cancellationToken))
        {
            return Result.Failure<Guid>(
                Error.Conflict(
                    "Category.DuplicateCode",
                    "A category with this code already exists."));
        }

        var category = Category.Create(request.Code, request.Icon);

        _categoryRepository.Add(category);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}