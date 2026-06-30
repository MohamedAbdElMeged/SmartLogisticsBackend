namespace SmartLogisticsBackend.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; }


    public static UserRole Create(Guid userId, Guid roleId)
    {
        return new UserRole()
        {
            UserId = userId,
            RoleId = roleId
        };
    }
}