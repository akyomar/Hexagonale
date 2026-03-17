using FluentAssertions;
using Hexagonal.Domain.ValueObjects;
using Xunit;

namespace Hexagonal.UnitTests.Domain.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void FromEur_WithPositiveAmount_CreatesMoney()
    {
        var m = Money.FromEur(10.50m);
        m.Amount.Should().Be(10.50m);
        m.Currency.Should().Be("EUR");
    }

    [Fact]
    public void FromEur_WithNegativeAmount_Throws()
    {
        var act = () => Money.FromEur(-1m);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Add_SameCurrency_ReturnsSum()
    {
        var a = Money.FromEur(10m);
        var b = Money.FromEur(5m);
        a.Add(b).Amount.Should().Be(15m);
    }

    [Fact]
    public void Multiply_ReturnsCorrectAmount()
    {
        Money.FromEur(10m).Multiply(3).Amount.Should().Be(30m);
    }
}
