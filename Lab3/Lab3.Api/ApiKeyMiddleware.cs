using System.Text.Json;
public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyHeader = "X-Api-Key";
    private const string ValidKey = "valid-test-key";

    public ApiKeyMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var key))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }
        if (key != ValidKey)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }
        await _next(context);
    }
}
