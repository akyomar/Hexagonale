using FluentAssertions;
using Hexagonal.Domain.Entities;
using Hexagonal.Domain.ValueObjects;
using Xunit;

namespace Hexagonal.UnitTests.Domain.Entities;

public class CustomerTests
{
    [Fact]
    public void Create_WithValidData_ReturnsCustomer()
    {
        var email = Email.Create("client@example.com");
        var customer = Customer.Create("Acme Corp", email);

        customer.Id.Should().NotBe(Guid.Empty);
        customer.Name.Should().Be("Acme Corp");
        customer.Email.Value.Should().Be("client@example.com");
        customer.Address.Should().BeNull();
    }

    [Fact]
    public void Create_WithAddress_SetsAddress()
    {
        var email = Email.Create("client@example.com");
        var address = new Address("1 rue Test", "Paris", "75001", "France");
        var customer = Customer.Create("Acme", email, address);

        customer.Address.Should().NotBeNull();
        customer.Address!.City.Should().Be("Paris");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_Throws(string? name)
    {
        var email = Email.Create("a@b.com");
        var act = () => Customer.Create(name!, email);
        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Fact]
    public void Rename_UpdatesName()
    {
        var customer = Customer.Create("Old", Email.Create("x@y.com"));
        customer.Rename("New Name");
        customer.Name.Should().Be("New Name");
    }
}
