using FluentAssertions;
using Hexagonal.Domain.Entities;
using Hexagonal.Domain.ValueObjects;
using Xunit;

namespace Hexagonal.UnitTests.Domain.Entities;

public class InvoiceTests
{
    [Fact]
    public void Create_WithValidData_ReturnsInvoice()
    {
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var invoice = Invoice.Create("FACT-2024-0001", orderId, customerId, Money.FromEur(100m));

        invoice.Id.Should().NotBe(Guid.Empty);
        invoice.InvoiceNumber.Should().Be("FACT-2024-0001");
        invoice.OrderId.Should().Be(orderId);
        invoice.CustomerId.Should().Be(customerId);
        invoice.TotalAmount.Amount.Should().Be(100m);
        invoice.Status.Should().Be(InvoiceStatus.Issued);
    }

    [Fact]
    public void Create_NormalizesInvoiceNumber()
    {
        var invoice = Invoice.Create("  fact-1  ", Guid.NewGuid(), Guid.NewGuid(), Money.FromEur(1m));
        invoice.InvoiceNumber.Should().Be("FACT-1");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyInvoiceNumber_Throws(string? number)
    {
        var act = () => Invoice.Create(number!, Guid.NewGuid(), Guid.NewGuid(), Money.FromEur(10m));
        act.Should().Throw<ArgumentException>().WithParameterName("invoiceNumber");
    }

    [Fact]
    public void Create_WithZeroAmount_Throws()
    {
        var act = () => Invoice.Create("F1", Guid.NewGuid(), Guid.NewGuid(), Money.FromEur(0));
        act.Should().Throw<ArgumentException>().WithParameterName("totalAmount");
    }

    [Fact]
    public void Create_WithNegativeAmount_Throws()
    {
        var act = () => Invoice.Create("F1", Guid.NewGuid(), Guid.NewGuid(), Money.FromEur(-1));
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MarkAsPaid_SetsStatusPaid()
    {
        var invoice = Invoice.Create("F1", Guid.NewGuid(), Guid.NewGuid(), Money.FromEur(50m));
        invoice.MarkAsPaid();
        invoice.Status.Should().Be(InvoiceStatus.Paid);
    }
}
