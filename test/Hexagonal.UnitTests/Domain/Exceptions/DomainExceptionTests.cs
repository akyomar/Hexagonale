using FluentAssertions;
using Hexagonal.Domain.Exceptions;
using Xunit;

namespace Hexagonal.UnitTests.Domain.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void DomainException_SetsMessageAndCode()
    {
        var ex = new DomainException("Test message", "CODE");
        ex.Message.Should().Be("Test message");
        ex.Code.Should().Be("CODE");
    }

    [Fact]
    public void NotFoundException_SetsEntityNameAndKey()
    {
        var key = Guid.NewGuid();
        var ex = new NotFoundException("Customer", key);
        ex.EntityName.Should().Be("Customer");
        ex.Key.Should().Be(key);
        ex.Code.Should().Be("NOT_FOUND");
    }
}
