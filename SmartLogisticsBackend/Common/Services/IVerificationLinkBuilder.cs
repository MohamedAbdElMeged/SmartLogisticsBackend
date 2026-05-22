namespace SmartLogisticsBackend.Common.Services;

public interface IVerificationLinkBuilder
{
    string BuildEmailVerificationLink(string email, string rawToken,string resource);

}