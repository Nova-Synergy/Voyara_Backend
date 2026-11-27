using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Domain.Repositories.Catalog;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Catalog.Commands.DeactivateCategory;

/// <summary>Deactivates a category so its destinations no longer appear in public search results.</summary>
public sealed class DeactivateCategoryCommandHandler
    : IRequestHandler<DeactivateCategoryCommand, Result>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Initializes the handler.</summary>
    /// <param name="categoryRepository">Repository used to load the category.</param>
    /// <param name="unitOfWork">Unit of work used to persist the deactivation and invalidate search cache.</param>
    public DeactivateCategoryCommandHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(
        DeactivateCategoryCommand request,
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

        category.Deactivate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}