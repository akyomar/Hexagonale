namespace Hexagonal.API.Models;

public class ErrorResponse
{
    public string Code { get; set; } = null!;
    public string? Message { get; set; }
}
