using System.Security.Claims;

namespace SmartLogisticsBackend.Common.Abstractions;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string email);
    ClaimsPrincipal? ValidateToken(string token);
}