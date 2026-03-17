namespace Hexagonal.Application.DTOs.Orders;

public record CreateOrderRequest
{
    public required Guid CustomerId { get; init; }
    public required string OrderNumber { get; init; }
    public required IReadOnlyList<OrderLineRequest> Lines { get; init; }
}

public record OrderLineRequest
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
