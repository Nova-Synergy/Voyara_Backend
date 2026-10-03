using System.Reflection;
using Voyara.SharedKernel.Results;

namespace Voyara.Application.Behaviors;

/// <summary>
/// Builds failed <see cref="Result"/>/<see cref="Result{TValue}"/> responses for the
/// concrete request response type, since pipeline behaviors are open-generic.
/// </summary>
internal static class PipelineResultFactory
{
    private static readonly MethodInfo FailureMethod = typeof(Result)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Single(m => m.Name == nameof(Result.Failure) && m.IsGenericMethodDefinition);

    /// <summary>Creates a failed response of type <typeparamref name="TResponse"/> carrying <paramref name="error"/>.</summary>
    public static TResponse CreateFailure<TResponse>(Error error)
    {
        if (typeof(TResponse) == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(error);
        }

        var valueType = typeof(TResponse).GetGenericArguments()[0];
        return (TResponse)FailureMethod.MakeGenericMethod(valueType).Invoke(null, [error])!;
    }
}
