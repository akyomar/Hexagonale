using Hexagonal.Domain.Common;
using Hexagonal.Domain.ValueObjects;

namespace Hexagonal.Domain.Entities;

/// <summary>
/// Entité métier Client.
/// </summary>
public class Customer : EntityBase
{
    public string Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Address? Address { get; private set; }

    private readonly List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders.AsReadOnly();

    private Customer() { } // EF Core

    private Customer(Guid id, string name, Email email, Address? address) : base(id)
    {
        Name = name;
        Email = email;
        Address = address;
    }

    public static Customer Create(string name, Email email, Address? address = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Le nom du client est requis.", nameof(name));

        return new Customer(Guid.NewGuid(), name.Trim(), email, address);
    }

    public void UpdateAddress(Address address)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Le nom du client est requis.", nameof(name));
        Name = name.Trim();
    }
}
