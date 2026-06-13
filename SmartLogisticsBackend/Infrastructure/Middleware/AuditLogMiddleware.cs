using System.Security.Claims;
using SmartLogisticsBackend.Domain.Entities;
using SmartLogisticsBackend.Infrastructure.Persistence;

namespace SmartLogisticsBackend.Infrastructure.Middleware;

public class AuditLogMiddleware(RequestDelegate next)
{
    private static readonly HashSet<string> _excluded =
    [
        "/health",
        "/metrics",
        "/favicon.ico",
    ];
 
    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
       
        if (_excluded.Contains(context.Request.Path.Value ?? string.Empty))
        {
            await next(context);
            return;
        }
 
        await next(context);
 
        try
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role   = context.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
 
            var log = AuditLog.Create(
                userId     : userId is null ? null : Guid.Parse(userId),
                role       : role,
                method     : context.Request.Method,
                path       : context.Request.Path.Value ?? string.Empty,
                statusCode : context.Response.StatusCode,
                ipAddress  : GetIpAddress(context),
                userAgent  : context.Request.Headers.UserAgent.ToString()
            );
 
            dbContext.AuditLogs.Add(log);
            await dbContext.SaveChangesAsync();
        }
        catch
        {
            // Audit failure must never affect the response the client already received
        }
    }
 
    private static string GetIpAddress(HttpContext context)
    {
        
        var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwarded))
            return forwarded.Split(',')[0].Trim();
 
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}