using MediatR;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Abstractions.Messaging;

/// <summary>A command that returns only success or an expected business error.</summary>
public interface ICommand : IRequest<Result>, IBaseCommand
{
}

/// <summary>A command that returns a value or an expected business error.</summary>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{
}
