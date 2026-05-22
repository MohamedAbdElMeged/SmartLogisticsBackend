using Hangfire;
using Microsoft.EntityFrameworkCore;
using SmartLogisticsBackend.Common.Services;
using SmartLogisticsBackend.Infrastructure.BackgroundJobs;
using SmartLogisticsBackend.Infrastructure.Persistence;

namespace SmartLogisticsBackend.Features.Users.ResendVerification;

public class ResendVerificationHandler(ApplicationDbContext context,
    IVerificationLinkBuilder verificationLinkBuilder,
    IBackgroundJobClient jobs)
{
    public async Task<Result> HandleAsync(ResendVerificationRequest req, CancellationToken ct)
    {
        
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email == req.Email.ToLower(), ct);

        if (user is null)
            return Result.Success();

        if (user.EmailVerified)
            return Result.Failure("Email is already verified.", ResultErrorType.Conflict);

        if (user.EmailVerificationTokenHash is not null && user.EmailVerificationExpiresAt > DateTime.UtcNow)
        {
            return Result.Failure(
                "A verification email was already sent. Please check your inbox or wait before requesting a new one.",
                ResultErrorType.Conflict);
        }   
        
        var rawToken = user.GenerateVerificationToken();

        await context.SaveChangesAsync(ct);

        var verificationLink = verificationLinkBuilder.BuildEmailVerificationLink(user.Email, rawToken, "users");
        jobs.Enqueue<EmailJob>(x =>
            x.SendVerificationEmailAfterRegistration(user.Email, user.FirstName, verificationLink));

        return Result.Success();
    }
}