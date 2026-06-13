namespace SmartLogisticsBackend.Domain.Entities;

public class AuditLog
{
    public Guid   Id          { get; private set; }
    public Guid?  UserId      { get; private set; }
    public string Role        { get; private set; } = string.Empty;
    public string Method      { get; private set; } = string.Empty;
    public string Path        { get; private set; } = string.Empty;
    public int    StatusCode  { get; private set; }
    public string IpAddress   { get; private set; } = string.Empty;
    public string UserAgent   { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    
    private AuditLog() { } 
 
    public static AuditLog Create(
        Guid?   userId,
        string  role,
        string  method,
        string  path,
        int     statusCode,
        string  ipAddress,
        string  userAgent) => new()
    {
        Id         = Guid.NewGuid(),
        UserId     = userId,
        Role       = role,
        Method     = method,
        Path       = path,
        StatusCode = statusCode,
        IpAddress  = ipAddress,
        UserAgent  = userAgent,
        CreatedAt  = DateTime.UtcNow,
    };
}