using FluentAssertions;
using Hexagonal.Application.DTOs.Products;
using Hexagonal.Application.Validation;
using Xunit;

namespace Hexagonal.UnitTests.Application.Validation;

public class CreateProductRequestValidatorTests
{
    private readonly CreateProductRequestValidator _validator = new();

    [Fact]
    public void ValidRequest_ShouldNotHaveErrors()
    {
        var request = new CreateProductRequest { Code = "P1", Name = "Product", UnitPriceEur = 10m };
        var result = _validator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void EmptyCode_ShouldHaveError()
    {
        var request = new CreateProductRequest { Code = "", Name = "Product", UnitPriceEur = 0 };
        var result = _validator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Code");
    }

    [Fact]
    public void NegativePrice_ShouldHaveError()
    {
        var request = new CreateProductRequest { Code = "P1", Name = "Product", UnitPriceEur = -1m };
        var result = _validator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UnitPriceEur");
    }
}
