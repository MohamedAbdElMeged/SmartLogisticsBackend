using Microsoft.EntityFrameworkCore;
using SmartLogisticsBackend.Infrastructure.Persistence;

namespace SmartLogisticsBackend.Features.Users.VerifyUser;

public class VerifyUserHandler(
    ApplicationDbContext context
    )
{
    public async Task<Result> HandleAsync(
        Request req,
        CancellationToken ct = default)
    {
        
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == req.Email, ct);
        if (user is null)
            return Result.Failure("Invalid token or user");


        var result = user.VerifyEmail(req.Token);
        if (!result.IsSuccess) return result;
        
        await context.SaveChangesAsync(ct);
        return Result.Success();

    }
}