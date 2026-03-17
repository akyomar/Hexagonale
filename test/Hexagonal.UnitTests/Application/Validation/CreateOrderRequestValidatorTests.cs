using FluentAssertions;
using Hexagonal.Application.DTOs.Orders;
using Hexagonal.Application.Validation;
using Xunit;

namespace Hexagonal.UnitTests.Application.Validation;

public class CreateOrderRequestValidatorTests
{
    private readonly CreateOrderRequestValidator _validator = new();

    [Fact]
    public void ValidRequest_ShouldNotHaveErrors()
    {
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid(),
            OrderNumber = "ORD-001",
            Lines = new List<OrderLineRequest> { new() { ProductId = Guid.NewGuid(), Quantity = 2 } }
        };
        var result = _validator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void EmptyOrderNumber_ShouldHaveError()
    {
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid(),
            OrderNumber = "",
            Lines = new List<OrderLineRequest> { new() { ProductId = Guid.NewGuid(), Quantity = 1 } }
        };
        var result = _validator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrderNumber");
    }

    [Fact]
    public void EmptyLines_ShouldHaveError()
    {
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid(),
            OrderNumber = "ORD-001",
            Lines = new List<OrderLineRequest>()
        };
        var result = _validator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Lines");
    }

    [Fact]
    public void LineWithZeroQuantity_ShouldHaveError()
    {
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid(),
            OrderNumber = "ORD-001",
            Lines = new List<OrderLineRequest> { new() { ProductId = Guid.NewGuid(), Quantity = 0 } }
        };
        var result = _validator.Validate(request);
        result.IsValid.Should().BeFalse();
    }
}
