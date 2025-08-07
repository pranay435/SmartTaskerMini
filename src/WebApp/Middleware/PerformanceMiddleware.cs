using System.Diagnostics;

namespace SmartTaskerMini.WebApp.Middleware;

public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMiddleware> _logger;

    public PerformanceMiddleware(RequestDelegate next, ILogger<PerformanceMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        await _next(context);
        
        stopwatch.Stop();
        var responseTime = stopwatch.ElapsedMilliseconds;
        
        if (responseTime > 1000) // Log slow requests
        {
            _logger.LogWarning("Slow request: {Method} {Path} took {ResponseTime}ms", 
                context.Request.Method, 
                context.Request.Path, 
                responseTime);
        }
        
        if (!context.Response.HasStarted)
        {
            context.Response.Headers["X-Response-Time"] = $"{responseTime}ms";
        }
    }
}