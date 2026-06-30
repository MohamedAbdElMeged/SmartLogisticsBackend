using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SmartLogisticsBackend.Common.Abstractions;
using SmartLogisticsBackend.Infrastructure.Persistence;

namespace SmartLogisticsBackend.Features.Users.SwitchRole;

public class SwitchRoleHandler(ApplicationDbContext context, IJwtTokenService tokenService)
{
    public async Task<Result<SwitchRoleResponse>> HandleAsync(SwitchRoleRequest req, ClaimsPrincipal currentUser, CancellationToken ct)
    {
        if (req.RoleName == currentUser.FindFirst(ClaimTypes.Role)?.Value)
        {
            return Result<SwitchRoleResponse>.Failure("You are already in this role");
        }
        var userId = Guid.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstAsync(u => u.Id == userId);
        var roles =  user.GetRolesNames();
        if (!roles.Contains(req.RoleName))
        {
            return Result<SwitchRoleResponse>.Failure("Invalid role");
        }
        var remainingRoles = roles.Where(r => r != req.RoleName).ToList();
        return Result<SwitchRoleResponse>.Success(new SwitchRoleResponse(tokenService.GenerateToken(user.Id,user.Email, req.RoleName, remainingRoles)));
            
    }
}