namespace Hexagonal.Domain.Exceptions;

/// <summary>
/// Levée lorsqu'une entité métier n'est pas trouvée.
/// </summary>
public class NotFoundException : DomainException
{
    public string EntityName { get; }
    public object? Key { get; }

    public NotFoundException(string entityName, object? key = null, string? message = null)
        : base(message ?? $"L'entité '{entityName}' n'a pas été trouvée.", "NOT_FOUND")
    {
        EntityName = entityName;
        Key = key;
    }
}
