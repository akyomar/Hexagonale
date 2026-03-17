using FluentAssertions;
using Hexagonal.Domain.Entities;
using Hexagonal.Domain.ValueObjects;
using Xunit;

namespace Hexagonal.UnitTests.Domain.Entities;

public class OrderItemTests
{
    [Fact]
    public void Create_WithValidData_ReturnsOrderItem()
    {
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var item = OrderItem.Create(orderId, productId, "P1", "Product 1", 3, Money.FromEur(10m));

        item.OrderId.Should().Be(orderId);
        item.ProductId.Should().Be(productId);
        item.ProductCode.Should().Be("P1");
        item.ProductName.Should().Be("Product 1");
        item.Quantity.Should().Be(3);
        item.UnitPrice.Amount.Should().Be(10m);
        item.LineTotal.Amount.Should().Be(30m);
    }

    [Fact]
    public void Create_WithZeroQuantity_Throws()
    {
        var act = () => OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "P1", "P", 0, Money.FromEur(1m));
        act.Should().Throw<ArgumentException>().WithParameterName("quantity");
    }

    [Fact]
    public void Create_WithNegativeQuantity_Throws()
    {
        var act = () => OrderItem.Create(Guid.NewGuid(), Guid.NewGuid(), "P1", "P", -1, Money.FromEur(1m));
        act.Should().Throw<ArgumentException>();
    }
}
