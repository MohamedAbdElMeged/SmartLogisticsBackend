namespace SmartLogisticsBackend.Common.Abstractions;

public interface IEmailSender
{
    Task SendVerificationEmailAsync(
        string email,
        string firstName,
        string verificationLink);
}