namespace Hexagonal.Application.DTOs.Customers;

public record CreateCustomerRequest
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public string? Street { get; init; }
    public string? City { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }
}
