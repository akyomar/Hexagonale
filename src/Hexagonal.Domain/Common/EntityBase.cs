namespace Hexagonal.Domain.Common;

/// <summary>
/// Base pour toutes les entités du domaine.
/// Fournit l'identité et l'égalité par Id.
/// </summary>
public abstract class EntityBase
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;

    protected EntityBase() { }

    protected EntityBase(Guid id)
    {
        Id = id;
    }

    public override bool Equals(object? obj) => obj is EntityBase other && Id == other.Id;
    public override int GetHashCode() => Id.GetHashCode();
}
