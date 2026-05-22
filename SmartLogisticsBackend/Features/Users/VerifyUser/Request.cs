namespace SmartLogisticsBackend.Features.Users.VerifyUser;

public record Request
{
    public string Token { get; set; }
    public string Email { get; set; }
}