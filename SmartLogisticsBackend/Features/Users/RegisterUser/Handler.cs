using Hangfire;
using Microsoft.EntityFrameworkCore;
using SmartLogisticsBackend.Common;
using SmartLogisticsBackend.Common.Services;
using SmartLogisticsBackend.Domain.Entities;
using SmartLogisticsBackend.Infrastructure.BackgroundJobs;
using SmartLogisticsBackend.Infrastructure.Persistence;

namespace SmartLogisticsBackend.Features.Users.RegisterUser;


public class RegisterUserHandler(
    ApplicationDbContext context,
    IVerificationLinkBuilder verificationLinkBuilder,
    IBackgroundJobClient jobs)
{
    public async Task<Result<RegisterResponse>> HandleAsync(
        RegisterRequest req,
        CancellationToken ct = default)
    {
        var emailTaken = await context.Users
            .AnyAsync(u => u.Email == req.Email.ToLower() , ct);

        if (emailTaken )
            return Result<RegisterResponse>.Conflict("Email is already registered.");
        
        var user = User.Create(req.FirstName, req.LastName, req.Email, req.Password);
        var rawToken = user.GenerateVerificationToken();
        await context.Users.AddAsync(user, ct);
        await context.SaveChangesAsync(ct);


        var verificationLink = verificationLinkBuilder.BuildEmailVerificationLink(user.Email, rawToken, "users");
        jobs.Enqueue<EmailJob>(x =>
            x.SendVerificationEmailAfterRegistration(user.Email, user.FirstName, verificationLink));
        
        return Result<RegisterResponse>.Success(new RegisterResponse
        {
            Id        = user.Id,
            Email     = user.Email,
            FirstName = user.FirstName,
            LastName  = user.LastName
            
        });
    }
}