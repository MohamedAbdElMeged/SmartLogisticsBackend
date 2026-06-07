namespace SmartLogisticsBackend.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
    }
}