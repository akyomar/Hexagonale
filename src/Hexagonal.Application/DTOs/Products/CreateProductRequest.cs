namespace Hexagonal.Application.DTOs.Products;

public record CreateProductRequest
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public decimal UnitPriceEur { get; init; }
}
