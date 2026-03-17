namespace Hexagonal.Application.DTOs.Orders;

public record OrderDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string OrderNumber { get; init; } = null!;
    public string Status { get; init; } = null!;
    public decimal TotalAmountEur { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public IReadOnlyList<OrderLineDto> Lines { get; init; } = Array.Empty<OrderLineDto>();
}

public record OrderLineDto
{
    public Guid ProductId { get; init; }
    public string ProductCode { get; init; } = null!;
    public string ProductName { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal UnitPriceEur { get; init; }
    public decimal LineTotalEur { get; init; }
}
