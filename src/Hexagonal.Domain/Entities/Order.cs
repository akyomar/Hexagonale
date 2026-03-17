using Hexagonal.Domain.Common;
using Hexagonal.Domain.ValueObjects;

namespace Hexagonal.Domain.Entities;

/// <summary>
/// Entité métier Commande.
/// </summary>
public class Order : EntityBase
{
    public Guid CustomerId { get; private set; }
    public string OrderNumber { get; private set; } = null!;
    public OrderStatus Status { get; private set; } = OrderStatus.Draft;

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public Money TotalAmount => _items.Aggregate(Money.Zero(), (acc, item) => acc.Add(item.LineTotal));

    private Order() { } // EF Core

    private Order(Guid id, Guid customerId, string orderNumber) : base(id)
    {
        CustomerId = customerId;
        OrderNumber = orderNumber;
    }

    public static Order Create(Guid customerId, string orderNumber)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException("Le numéro de commande est requis.", nameof(orderNumber));

        return new Order(Guid.NewGuid(), customerId, orderNumber.Trim().ToUpperInvariant());
    }

    public void AddItem(OrderItem item)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Seules les commandes en brouillon peuvent être modifiées.");
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        _items.Add(item);
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException("Seule une commande en brouillon peut être confirmée.");
        if (_items.Count == 0)
            throw new InvalidOperationException("Impossible de confirmer une commande sans lignes.");
        Status = OrderStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped)
            throw new InvalidOperationException("Une commande expédiée ne peut pas être annulée.");
        Status = OrderStatus.Cancelled;
    }
}

public enum OrderStatus
{
    Draft = 0,
    Confirmed = 1,
    Shipped = 2,
    Cancelled = 3
}
