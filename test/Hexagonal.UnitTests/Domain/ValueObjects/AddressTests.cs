using FluentAssertions;
using Hexagonal.Domain.ValueObjects;
using Xunit;

namespace Hexagonal.UnitTests.Domain.ValueObjects;

public class AddressTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var addr = new Address("1 rue Test", "Paris", "75001", "France");
        addr.Street.Should().Be("1 rue Test");
        addr.City.Should().Be("Paris");
        addr.PostalCode.Should().Be("75001");
        addr.Country.Should().Be("France");
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        var a = new Address("S", "C", "P", "Co");
        var b = new Address("S", "C", "P", "Co");
        a.Equals(b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        var a = new Address("S1", "C", "P", "Co");
        var b = new Address("S2", "C", "P", "Co");
        a.Equals(b).Should().BeFalse();
    }
}
