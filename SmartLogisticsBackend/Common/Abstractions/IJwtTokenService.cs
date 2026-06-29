using System.Security.Claims;

namespace SmartLogisticsBackend.Common.Abstractions;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string email, string activeRole, List<string> roles);
    ClaimsPrincipal? ValidateToken(string token);
}