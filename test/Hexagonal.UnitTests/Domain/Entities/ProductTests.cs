using FluentAssertions;
using Hexagonal.Domain.Entities;
using Hexagonal.Domain.ValueObjects;
using Xunit;

namespace Hexagonal.UnitTests.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_ReturnsProduct()
    {
        var product = Product.Create("P001", "Product One", Money.FromEur(19.99m), "Description");

        product.Id.Should().NotBe(Guid.Empty);
        product.Code.Should().Be("P001");
        product.Name.Should().Be("Product One");
        product.Description.Should().Be("Description");
        product.UnitPrice.Amount.Should().Be(19.99m);
        product.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_NormalizesCodeToUpper()
    {
        var product = Product.Create("abc", "Name", Money.FromEur(0));
        product.Code.Should().Be("ABC");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCode_Throws(string? code)
    {
        var act = () => Product.Create(code!, "Name", Money.FromEur(1));
        act.Should().Throw<ArgumentException>().WithParameterName("code");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_WithEmptyName_Throws(string? name)
    {
        var act = () => Product.Create("P1", name!, Money.FromEur(1));
        act.Should().Throw<ArgumentException>().WithParameterName("name");
    }

    [Fact]
    public void Create_WithNegativePrice_Throws()
    {
        var act = () => Product.Create("P1", "Name", Money.FromEur(-1));
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdatePrice_UpdatesUnitPrice()
    {
        var product = Product.Create("P1", "Name", Money.FromEur(10m));
        product.UpdatePrice(Money.FromEur(15m));
        product.UnitPrice.Amount.Should().Be(15m);
    }

    [Fact]
    public void UpdatePrice_WithNegative_Throws()
    {
        var product = Product.Create("P1", "Name", Money.FromEur(10m));
        var act = () => product.UpdatePrice(Money.FromEur(-1m));
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var product = Product.Create("P1", "Name", Money.FromEur(1));
        product.Deactivate();
        product.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_SetsIsActiveTrue()
    {
        var product = Product.Create("P1", "Name", Money.FromEur(1));
        product.Deactivate();
        product.Activate();
        product.IsActive.Should().BeTrue();
    }
}
