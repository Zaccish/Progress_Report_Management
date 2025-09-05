namespace ProgressReportSystem.API.Middleware;

public class ActivityLoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ActivityLoggerMiddleware> _logger;

    public ActivityLoggerMiddleware(RequestDelegate next, ILogger<ActivityLoggerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.User.Identity?.Name ?? "Anonymous";
        var path = context.Request.Path;

        _logger.LogInformation($"[Activity] {user} accessed {path} at {DateTime.UtcNow}");

        await _next(context);
    }
}
