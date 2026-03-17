using FluentAssertions;
using Hexagonal.Application.Common;
using Xunit;

namespace Hexagonal.UnitTests.Application.Common;

public class ResultTests
{
    [Fact]
    public void Success_ReturnsSuccessResult()
    {
        var result = Result.Success();
        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.ErrorCode.Should().BeNull();
    }

    [Fact]
    public void Failure_ReturnsFailureResult()
    {
        var result = Result.Failure("Something failed", "ERR");
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Something failed");
        result.ErrorCode.Should().Be("ERR");
    }

    [Fact]
    public void ResultT_Success_ReturnsValue()
    {
        var result = Result<int>.Success(42);
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
        result.GetValueOrThrow().Should().Be(42);
    }

    [Fact]
    public void ResultT_Failure_GetValueOrThrow_Throws()
    {
        var result = Result<int>.Failure("Failed", "ERR");
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().Be(default);
        var act = () => result.GetValueOrThrow();
        act.Should().Throw<InvalidOperationException>().WithMessage("*Failed*");
    }
}
