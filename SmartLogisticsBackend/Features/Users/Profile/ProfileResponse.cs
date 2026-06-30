namespace SmartLogisticsBackend.Features.Users.Profile;

public record ProfileResponse(Guid Id, string Email, string FirstName, string LastName, List<string> Roles);