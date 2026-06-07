using Hangfire;
using Microsoft.AspNetCore.Identity;
using Resend;
using SmartLogisticsBackend.Common.Abstractions;

namespace SmartLogisticsBackend.Infrastructure.Email;

public class EmailSender : IEmailSender
{
    private readonly IResend _resend;
    private readonly IWebHostEnvironment _env;

    public EmailSender(IResend resend, IWebHostEnvironment env)
    {
        _resend = resend;
        _env = env;
    }
    public async Task SendVerificationEmailAsync(
        string email,
        string firstName,
        string verificationLink)
    {
        var htmlPath = Path.Combine(_env.ContentRootPath, "Infrastructure", "Email", "Templates", "VerifyEmail.html");

        var html = await File.ReadAllTextAsync(htmlPath);
        html = html
            .Replace("{{FirstName}}", firstName)
            .Replace("{{VerificationLink}}", verificationLink);
        var message = new EmailMessage();
        message.From = "onboarding@resend.dev";
        message.To.Add( email );
        message.Subject = "Smart Logistics - Verify Your Email";
        message.HtmlBody = html;
        await _resend.EmailSendAsync( message );
    }
    
}