using Hexagonal.Domain.Common;
using Hexagonal.Domain.ValueObjects;

namespace Hexagonal.Domain.Entities;

/// <summary>
/// Ligne de commande : produit + quantité + prix au moment de la commande.
/// </summary>
public class OrderItem : EntityBase
{
    public Guid ProductId { get; private set; }
    public string ProductCode { get; private set; } = null!;
    public string ProductName { get; private set; } = null!;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public Money LineTotal => UnitPrice.Multiply(Quantity);

    public Guid OrderId { get; private set; }

    private OrderItem() { } // EF Core

    internal OrderItem(Guid orderId, Guid productId, string productCode, string productName, int quantity, Money unitPrice) : base(Guid.NewGuid())
    {
        OrderId = orderId;
        ProductId = productId;
        ProductCode = productCode;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public static OrderItem Create(Guid orderId, Guid productId, string productCode, string productName, int quantity, Money unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("La quantité doit être strictement positive.", nameof(quantity));
        return new OrderItem(orderId, productId, productCode, productName, quantity, unitPrice);
    }
}
