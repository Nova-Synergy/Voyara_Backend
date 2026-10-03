using FluentValidation;
using MediatR;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Behaviors;

/// <summary>Runs all FluentValidation validators for the request; failures become a validation <see cref="Result"/>.</summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>Creates the behavior.</summary>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = (await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(result => result.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
        {
            var error = Error.Validation(
                "Validation.General",
                string.Join(" | ", failures.Select(f => f.ErrorMessage)));
            return PipelineResultFactory.CreateFailure<TResponse>(error);
        }

        return await next();
    }
}
