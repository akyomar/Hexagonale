using FluentAssertions;
using Hexagonal.Application.DTOs.Customers;
using Hexagonal.Application.Validation;
using Xunit;

namespace Hexagonal.UnitTests.Application.Validation;

public class CreateCustomerRequestValidatorTests
{
    private readonly CreateCustomerRequestValidator _validator = new();

    [Fact]
    public void ValidRequest_ShouldNotHaveErrors()
    {
        var request = new CreateCustomerRequest { Name = "Acme", Email = "acme@test.com" };
        var result = _validator.Validate(request);
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyName_ShouldHaveError(string? name)
    {
        var request = new CreateCustomerRequest { Name = name!, Email = "a@b.com" };
        var result = _validator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid")]
    public void InvalidEmail_ShouldHaveError(string? email)
    {
        var request = new CreateCustomerRequest { Name = "Acme", Email = email! };
        var result = _validator.Validate(request);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }
}
