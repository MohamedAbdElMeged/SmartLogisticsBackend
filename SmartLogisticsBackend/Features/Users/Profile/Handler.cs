using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SmartLogisticsBackend.Infrastructure.Persistence;

namespace SmartLogisticsBackend.Features.Users.Profile;

public class GetProfileHandler(ApplicationDbContext context)
{
    
    public async Task<Result<ProfileResponse>> HandleAsync(ClaimsPrincipal currentUser, CancellationToken ct)
    {
        var userId = Guid.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await context.Users.Include(u=> u.UserRoles).ThenInclude(ur=> ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
        return user == null ? Result<ProfileResponse>.Failure("User not found") : Result<ProfileResponse>.Success(new ProfileResponse(user.Id, user.Email, user.FirstName, user.LastName, user.GetRolesNames()));
    }
}