using Microsoft.EntityFrameworkCore;
using SmartLogisticsBackend.Common.Abstractions;
using SmartLogisticsBackend.Infrastructure.Persistence;

namespace SmartLogisticsBackend.Features.Users.LoginUser;

public class LoginUserHandler(ApplicationDbContext context,
    IJwtTokenService tokenService)
{
    public async Task<Result<LoginUserResponse>> HandleAsync(LoginUserRequest req, CancellationToken ct)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == req.Email.ToLower(), ct);
        if (user is null)
            return Result<LoginUserResponse>.Unauthorized("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHashed))
        {
            user.IncrementFailedLoginAttempts();
            await context.SaveChangesAsync(ct);
            return Result<LoginUserResponse>.Unauthorized("Invalid email or password.");
        }

        if (user.LockedUntil > DateTime.UtcNow)
        {
            return Result<LoginUserResponse>.Locked("Account is locked.");
        }
        if (!user.EmailVerified)
        {
            return Result<LoginUserResponse>.Forbidden("Email is not verified.");
        }

        var token = tokenService.GenerateToken(user.Id, user.Email);
        user.ResetFailedLoginAttempts();
        await context.SaveChangesAsync(ct);
        return Result<LoginUserResponse>.Success(new LoginUserResponse()
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = token
        });
    }
}