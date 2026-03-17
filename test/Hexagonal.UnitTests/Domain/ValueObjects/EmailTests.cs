using FluentAssertions;
using Hexagonal.Domain.ValueObjects;
using Xunit;

namespace Hexagonal.UnitTests.Domain.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("a@b.com")]
    [InlineData("user@domain.fr")]
    [InlineData("User@Domain.COM")]
    public void Create_WithValidEmail_ReturnsEmail(string value)
    {
        var email = Email.Create(value);
        email.Value.Should().Be(value.Trim().ToLowerInvariant());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmpty_Throws(string? value)
    {
        var act = () => Email.Create(value!);
        act.Should().Throw<ArgumentException>().WithParameterName("value");
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("no-at-sign")]
    [InlineData("@nodomain.com")]
    [InlineData("nodomain@")]
    public void Create_WithInvalidFormat_Throws(string value)
    {
        var act = () => Email.Create(value);
        act.Should().Throw<ArgumentException>().WithParameterName("value");
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        Email.Create("x@y.com").ToString().Should().Be("x@y.com");
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        var a = Email.Create("a@b.com");
        var b = Email.Create("a@b.com");
        a.Equals(b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
