using SmartLogisticsBackend.Domain.ValueObjects;

namespace SmartLogisticsBackend.Domain.Models;

public class Order
{

    public int Id { get; set; }
    public int UserId { get; set; }
    public Money TotalPrice { get; set; }
}