namespace BlogApi.Api.Middlewares;

public class InterceptorMiddleware(RequestDelegate next, ILogger<InterceptorMiddleware> logger)
{

    private const string TrackerKey = "X-Tracker-ID";

    public async Task InvokeAsync(HttpContext context)
    {

        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await next(context);
            return;
        };
        var requestTime = DateTime.UtcNow;
        var request = context.Request;
        var method = request.Method;
        var path = request.Path;
        var trackerId = SetupTrackerId(context);

        var refBody = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            await next(context);
        }
        finally
        {

            var responseTime = DateTime.UtcNow;
            var duration = responseTime - requestTime;
            var statusCode = context.Response.StatusCode;

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var logMessage = FormatMessage(
                statusCode,
                requestTime,
                responseTime,
                duration,
                request,
                responseText,
                trackerId
            );

            logger.Log(LogLevel.Information, logMessage);
            await responseBodyStream.CopyToAsync(refBody);
        }
    }
    private string FormatMessage(int statusCode, DateTime requestTime,
        DateTime responseTime, TimeSpan duration, HttpRequest request, string responseText, string trackerId)
    {
        var headers = string.Join("\n", request.Headers.Select(h => $"\t {h.Key}: {h.Value}"));
        return $"""
                 =======================
                 HTTP Request Information
                 =======================
                 ID: {trackerId ?? "N/A"}
                 Timestamp: {requestTime:yyyy-MM-dd HH:mm:ss.fff}
                 HTTP Method: {request.Method}
                 Request Path: {request.Path}
                 Query String: {request.QueryString}
                 Headers: {"\n" + headers}
                 -----------------------
                 HTTP Response Information
                 -----------------------
                 Status Code: {statusCode}
                 Response Time: {responseTime:yyyy-MM-dd HH:mm:ss.fff}
                 Execution Duration: {duration.TotalMilliseconds} ms
                 Response: {responseText}
                 =======================
                 """;
    }
    private string SetupTrackerId(HttpContext context)
    {
        var trackerId = context.Request.Cookies[TrackerKey];
        if (trackerId is not null) return trackerId;
        var newTrackerId = Guid.NewGuid().ToString();
        context.Response.Cookies.Append(TrackerKey, newTrackerId, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddYears(1)
        });
        return newTrackerId;
    }
}