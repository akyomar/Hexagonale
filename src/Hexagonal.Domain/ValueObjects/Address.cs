namespace Hexagonal.Domain.ValueObjects;

/// <summary>
/// Value Object pour une adresse postale.
/// </summary>
public sealed class Address : IEquatable<Address>
{
    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; }

    public Address(string street, string city, string postalCode, string country)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
        PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
        Country = country ?? throw new ArgumentNullException(nameof(country));
    }

    public bool Equals(Address? other) =>
        other is not null &&
        Street == other.Street &&
        City == other.City &&
        PostalCode == other.PostalCode &&
        Country == other.Country;

    public override bool Equals(object? obj) => obj is Address other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Street, City, PostalCode, Country);
}
