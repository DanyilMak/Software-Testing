using System.Text.Json;
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);
        var correlationId = context.Items["CorrelationId"]?.ToString();
        _logger.LogInformation("Request {method} {path} => {status} CorrelationId={correlationId}",
            context.Request.Method, context.Request.Path, context.Response.StatusCode, correlationId);
    }
}
