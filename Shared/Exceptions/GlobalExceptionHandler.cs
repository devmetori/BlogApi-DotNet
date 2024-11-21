using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace BlogApi.Shared.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        
        if (exception is HttpException)
        {
            var exc = (HttpException)exception;
            var response = new
            {
                Success = false,
                Error = new { code = exc.Code, message = exc.Message, requestId = traceId }
            };
            logger.LogError(exception,exc.Message);
            
            await Results.Json(response).ExecuteAsync(httpContext);

            return true;
        }

        await Results.Json(new
        {
            success = false,
            error = new
            {
                code = 0,
                message = "Parece que nuestro servicio no esta disponible en este momento, por favor intenta mas tarde",
                requestId = traceId
            }
        }).ExecuteAsync(httpContext);

        return true;
    }

    private static (int StatusCode, string Title) MapException(Exception exception)
    {
        return exception switch
        {
            ArgumentOutOfRangeException => (StatusCodes.Status400BadRequest, exception.Message),

            _ => (StatusCodes.Status500InternalServerError, "We made a mistake but we are on it!")
        };
    }
}