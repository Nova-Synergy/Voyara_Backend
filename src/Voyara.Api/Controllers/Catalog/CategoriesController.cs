using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voyara.Api.Authorization;
using Voyara.Application.Catalog.Commands.CreateCategory;
using Voyara.Application.Catalog.Commands.UpdateCategory;
using Voyara.Application.Catalog.Commands.DeactivateCategory;
using Voyara.Application.Catalog.Queries.GetAllCategories;
using Voyara.Application.Catalog.Queries.GetCategoryById;

namespace Voyara.Api.Controllers.Catalog;

[Authorize(Policy = AuthorizationPolicies.AdminOnly)]
[Route("api/v1/admin/categories")]
public sealed class CategoriesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new GetAllCategoriesQuery(), cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new GetCategoryByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateCategoryCommand command,
        CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(command, cancellationToken));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match command ID.");

        return HandleResult(await Sender.Send(command, cancellationToken));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Deactivate(
        Guid id, CancellationToken cancellationToken)
        => HandleResult(await Sender.Send(
            new DeactivateCategoryCommand(id), cancellationToken));
}