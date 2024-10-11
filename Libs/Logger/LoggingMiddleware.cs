using System.Text;

namespace BlogApi.Libs.Logger;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

   

  

    public async Task InvokeAsync(HttpContext context)
    {
        var trackerId = context.Request.Headers["X-Tracker-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString();
        var request = await FormatRequest(context, trackerId);
        var originalBodyStream = context.Response.Body;
        using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;
            _logger.Log(LogLevel.Information,request);
            try
            {
                await _next(context);
            }
            finally
            {
                var response = await FormatResponse(context, trackerId);
                _logger.Log(LogLevel.Information, response);
            }

            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private async Task<string> FormatRequest(HttpContext context, string trackerId)
    {
        var request = context.Request;
        request.EnableBuffering();
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
        var text = Encoding.UTF8.GetString(buffer);
        request.Body.Position = 0;
        return $"[{trackerId}] {context.Request.Method} - {context.Connection.RemoteIpAddress}{context.Request.Path} - {context.Response.StatusCode} {text}";
    }

    private async Task<string> FormatResponse(HttpContext context, string trackerId)
    {
        var response = context.Response;
        response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        return $"[{trackerId}] {context.Request.Method} - {context.Connection.RemoteIpAddress}{context.Request.Path} - {context.Response.StatusCode} {text}";
    }
}