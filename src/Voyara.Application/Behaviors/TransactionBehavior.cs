using MediatR;
using Voyara.Application.Abstractions;
using Voyara.Application.Abstractions.Messaging;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Behaviors;

/// <summary>Wraps commands (<see cref="IBaseCommand"/>) in a unit-of-work transaction;
/// commits on success, rolls back on failure or exception.</summary>
public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Creates the behavior.</summary>
    public TransactionBehavior(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next();

            if (response is Result { IsFailure: true })
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            }
            else
            {
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }

            return response;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);
            throw;
        }
    }
}
