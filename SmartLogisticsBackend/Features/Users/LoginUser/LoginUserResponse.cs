namespace SmartLogisticsBackend.Features.Users.LoginUser;

public class LoginUserResponse
{

    public Guid Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Token { get; set; }
  
}
