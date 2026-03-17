using FluentAssertions;
using Hexagonal.Application.Ports.Out;
using Hexagonal.Application.UseCases.Customers;
using Hexagonal.Domain.Entities;
using Hexagonal.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Hexagonal.UnitTests.Application.UseCases;

public class GetCustomerByIdUseCaseTests
{
    private readonly Mock<ICustomerRepository> _customerRepo;
    private readonly GetCustomerByIdUseCase _sut;

    public GetCustomerByIdUseCaseTests()
    {
        _customerRepo = new Mock<ICustomerRepository>();
        _sut = new GetCustomerByIdUseCase(_customerRepo.Object, Mock.Of<ILogger<GetCustomerByIdUseCase>>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenCustomerExists_ReturnsCustomerDto()
    {
        var customer = Customer.Create("Test", Email.Create("t@t.com"));
        _customerRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(customer);

        var result = await _sut.ExecuteAsync(customer.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(customer.Id);
        result.Value.Name.Should().Be("Test");
        result.Value.Email.Should().Be("t@t.com");
    }

    [Fact]
    public async Task ExecuteAsync_WhenCustomerNotFound_ReturnsFailure()
    {
        _customerRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Customer?)null);

        var result = await _sut.ExecuteAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("NOT_FOUND");
    }
}
