using FastEndpoints;
using StrawberryShake;

namespace MamisSolidarias.WebAPI.Campaigns.Extensions;

internal static class IOperationResultExtensions
{
    public static bool HasAuthError<T>(this IOperationResult<T> result) where T : class 
        => result.Errors.Any(t => t.Code is "AUTH_NOT_AUTHORIZED");

    public static async Task<bool> HandleErrors<T>(this IOperationResult<T> result,
        Func<CancellationToken,Task>? onAuthError, 
        Func<CancellationToken,Task>? onOtherError,
        CancellationToken token
        ) 
        where T : class
    {
        
        if (result.HasAuthError() && onAuthError is not null)
        {
            await onAuthError.Invoke(token);
            return true;
        }
        
        if (result.IsErrorResult() && onOtherError is not null)
        {
            await onOtherError.Invoke(token);
            return true;
        }
        

        return false;
    }
}