namespace SmartLogisticsBackend.Domain.ValueObjects;

public record Money
{
    public double Amount { get; set; }
    public string Currency { get; set; }
}