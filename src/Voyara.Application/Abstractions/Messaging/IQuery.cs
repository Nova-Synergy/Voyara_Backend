using MediatR;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Abstractions.Messaging;

/// <summary>A side-effect-free request returning a value or an expected business error.</summary>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
