namespace SmartLogisticsBackend.Common.Services;

public class VerificationLinkBuilder(IConfiguration config) : IVerificationLinkBuilder
{
    public string BuildEmailVerificationLink(string email, string rawToken,string resource)
    {
        var baseUrl = config["AppSettings:BaseUrl"]
                      ?? throw new InvalidOperationException("AppSettings:BaseUrl not configured.");

        var token = Uri.EscapeDataString(rawToken);
        var encodedEmail = Uri.EscapeDataString(email);

        return $"{baseUrl}/{resource}/verify-email?token={token}&email={encodedEmail}";
    }
}