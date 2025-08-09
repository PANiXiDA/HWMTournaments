using System.Security.Claims;

namespace UI.Server.Middlewares;

public sealed class LoggingMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleWare> _logger;

    public LoggingMiddleWare(RequestDelegate next, ILogger<LoggingMiddleWare> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.User?.Identity?.IsAuthenticated ?? false)
        {
            string personLogin = context.User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            string personId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

            using (_logger.BeginScope("{personLogin}, {personId}", personLogin, personId))
            {
                await _next(context);
            }
        }
        else
        {
            await _next(context);
        }
    }
}