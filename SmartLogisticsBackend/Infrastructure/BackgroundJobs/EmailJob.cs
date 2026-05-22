using Hangfire;
using SmartLogisticsBackend.Common.Abstractions;

namespace SmartLogisticsBackend.Infrastructure.BackgroundJobs;

public class EmailJob(IEmailSender emailSender)
{
    [Queue("emails")]
    public async Task SendVerificationEmailAfterRegistration(string email, string firstName,string verificationLink)
    {
        
        await emailSender.SendVerificationEmailAsync(email, firstName, verificationLink);
    }
}