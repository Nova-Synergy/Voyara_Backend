using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Catalog;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Catalog.Commands.UpdateCategory;

/// <summary>Updates category metadata and visibility.</summary>
public sealed class UpdateCategoryCommandHandler
    : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the category update handler.</summary>
    /// <param name="categoryRepository">Repository used to load the category.</param>
    /// <param name="unitOfWork">Unit of work used to persist changes and invalidate affected search cache entries.</param>
    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(
            request.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure(
                Error.NotFound(
                    "Category.NotFound",
                    $"Category '{request.Id}' was not found."));
        }

        category.ChangeIcon(request.Icon);

        if (request.IsActive && !category.IsActive)
            category.Activate();

        else if (!request.IsActive && category.IsActive)
            category.Deactivate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}