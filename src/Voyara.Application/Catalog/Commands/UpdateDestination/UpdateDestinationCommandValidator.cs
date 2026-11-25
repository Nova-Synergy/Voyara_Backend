using FluentValidation;

namespace Voyara.Application.Catalog.Commands.UpdateDestination;

public sealed class UpdateDestinationCommandValidator : AbstractValidator<UpdateDestinationCommand>
{
    public UpdateDestinationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.CityId).GreaterThan(0);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.EstimatedVisitMinutes).InclusiveBetween(1, 1440);
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
    }
}