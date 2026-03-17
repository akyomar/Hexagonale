namespace Hexagonal.Domain.ValueObjects;

/// <summary>
/// Value Object représentant un montant monétaire (invariant, immutable).
/// </summary>
public sealed class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency ?? "EUR";
    }

    public static Money FromEur(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Le montant ne peut pas être négatif.", nameof(amount));
        return new Money(amount, "EUR");
    }

    public static Money Zero(string currency = "EUR") => new(0, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Impossible d'ajouter des montants en {Currency} et {other.Currency}.");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
    {
        if (factor < 0)
            throw new ArgumentException("Le facteur ne peut pas être négatif.", nameof(factor));
        return new Money(Amount * factor, Currency);
    }

    public bool Equals(Money? other) => other is not null && Amount == other.Amount && Currency == other.Currency;
    public override bool Equals(object? obj) => obj is Money other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
}
