using Hexagonal.Domain.Common;
using Hexagonal.Domain.ValueObjects;

namespace Hexagonal.Domain.Entities;

/// <summary>
/// Entité métier Facture.
/// </summary>
public class Invoice : EntityBase
{
    public string InvoiceNumber { get; private set; } = null!;
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Money TotalAmount { get; private set; } = null!;
    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Issued;

    private Invoice() { } // EF Core

    private Invoice(Guid id, string invoiceNumber, Guid orderId, Guid customerId, Money totalAmount) : base(id)
    {
        InvoiceNumber = invoiceNumber;
        OrderId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
    }

    public static Invoice Create(string invoiceNumber, Guid orderId, Guid customerId, Money totalAmount)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new ArgumentException("Le numéro de facture est requis.", nameof(invoiceNumber));
        if (totalAmount.Amount <= 0)
            throw new ArgumentException("Le montant total doit être strictement positif.", nameof(totalAmount));

        return new Invoice(Guid.NewGuid(), invoiceNumber.Trim().ToUpperInvariant(), orderId, customerId, totalAmount);
    }

    public void MarkAsPaid() => Status = InvoiceStatus.Paid;
}

public enum InvoiceStatus
{
    Issued = 0,
    Paid = 1,
    Cancelled = 2
}
