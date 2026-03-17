using Hexagonal.Domain.Common;
using Hexagonal.Domain.ValueObjects;

namespace Hexagonal.Domain.Entities;

/// <summary>
/// Entité métier Produit.
/// </summary>
public class Product : EntityBase
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    private Product() { } // EF Core

    private Product(Guid id, string code, string name, Money unitPrice, string? description) : base(id)
    {
        Code = code;
        Name = name;
        UnitPrice = unitPrice;
        Description = description;
    }

    public static Product Create(string code, string name, Money unitPrice, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Le code produit est requis.", nameof(code));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Le nom produit est requis.", nameof(name));
        if (unitPrice.Amount < 0)
            throw new ArgumentException("Le prix unitaire ne peut pas être négatif.", nameof(unitPrice));

        return new Product(Guid.NewGuid(), code.Trim().ToUpperInvariant(), name.Trim(), unitPrice, description?.Trim());
    }

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice.Amount < 0)
            throw new ArgumentException("Le prix ne peut pas être négatif.", nameof(newPrice));
        UnitPrice = newPrice;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
