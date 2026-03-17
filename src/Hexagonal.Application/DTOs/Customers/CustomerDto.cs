namespace Hexagonal.Application.DTOs.Customers;

public record CustomerDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? Street { get; init; }
    public string? City { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
