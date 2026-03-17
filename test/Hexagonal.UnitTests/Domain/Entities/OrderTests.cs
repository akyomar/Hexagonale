using FluentAssertions;
using Hexagonal.Domain.Entities;
using Hexagonal.Domain.ValueObjects;
using Xunit;

namespace Hexagonal.UnitTests.Domain.Entities;

public class OrderTests
{
    [Fact]
    public void Create_WithValidData_ReturnsOrder()
    {
        var customerId = Guid.NewGuid();
        var order = Order.Create(customerId, "ORD-001");

        order.Id.Should().NotBe(Guid.Empty);
        order.CustomerId.Should().Be(customerId);
        order.OrderNumber.Should().Be("ORD-001");
        order.Status.Should().Be(OrderStatus.Draft);
        order.Items.Should().BeEmpty();
        order.TotalAmount.Amount.Should().Be(0);
    }

    [Fact]
    public void Create_NormalizesOrderNumber()
    {
        var order = Order.Create(Guid.NewGuid(), "  ord-002  ");
        order.OrderNumber.Should().Be("ORD-002");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrderNumber_Throws(string? orderNumber)
    {
        var act = () => Order.Create(Guid.NewGuid(), orderNumber!);
        act.Should().Throw<ArgumentException>().WithParameterName("orderNumber");
    }

    [Fact]
    public void AddItem_AddsItemAndUpdatesTotal()
    {
        var order = Order.Create(Guid.NewGuid(), "ORD-001");
        var item = OrderItem.Create(order.Id, Guid.NewGuid(), "P1", "Product 1", 2, Money.FromEur(10m));

        order.AddItem(item);

        order.Items.Should().HaveCount(1);
        order.TotalAmount.Amount.Should().Be(20m);
    }

    [Fact]
    public void AddItem_WhenNotDraft_Throws()
    {
        var order = Order.Create(Guid.NewGuid(), "ORD-001");
        var item = OrderItem.Create(order.Id, Guid.NewGuid(), "P1", "P", 1, Money.FromEur(5m));
        order.AddItem(item);
        order.Confirm();

        var act = () => order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "P2", "P2", 1, Money.FromEur(1m)));
        act.Should().Throw<InvalidOperationException>().WithMessage("*brouillon*");
    }

    [Fact]
    public void Confirm_WithItems_ChangesStatusToConfirmed()
    {
        var order = Order.Create(Guid.NewGuid(), "ORD-001");
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "P1", "P", 1, Money.FromEur(10m)));

        order.Confirm();

        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void Confirm_WithoutItems_Throws()
    {
        var order = Order.Create(Guid.NewGuid(), "ORD-001");
        var act = () => order.Confirm();
        act.Should().Throw<InvalidOperationException>().WithMessage("*sans lignes*");
    }

    [Fact]
    public void Confirm_WhenAlreadyConfirmed_Throws()
    {
        var order = Order.Create(Guid.NewGuid(), "ORD-001");
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "P1", "P", 1, Money.FromEur(10m)));
        order.Confirm();

        var act = () => order.Confirm();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cancel_WhenDraft_SetsCancelled()
    {
        var order = Order.Create(Guid.NewGuid(), "ORD-001");
        order.Cancel();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenConfirmed_SetsCancelled()
    {
        var order = Order.Create(Guid.NewGuid(), "ORD-001");
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "P1", "P", 1, Money.FromEur(10m)));
        order.Confirm();
        order.Cancel();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void AddItem_WithNull_Throws()
    {
        var order = Order.Create(Guid.NewGuid(), "ORD-001");
        var act = () => order.AddItem(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
