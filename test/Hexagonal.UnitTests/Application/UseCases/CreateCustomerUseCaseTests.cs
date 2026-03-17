using FluentAssertions;
using Hexagonal.Application.DTOs.Customers;
using Hexagonal.Application.Ports.Out;
using Hexagonal.Application.UseCases.Customers;
using Hexagonal.Domain.Entities;
using Hexagonal.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Hexagonal.UnitTests.Application.UseCases;

public class CreateCustomerUseCaseTests
{
    private readonly Mock<ICustomerRepository> _customerRepo;
    private readonly Mock<IUnitOfWork> _uow;
    private readonly CreateCustomerUseCase _sut;

    public CreateCustomerUseCaseTests()
    {
        _customerRepo = new Mock<ICustomerRepository>();
        _uow = new Mock<IUnitOfWork>();
        _sut = new CreateCustomerUseCase(_customerRepo.Object, _uow.Object, Mock.Of<ILogger<CreateCustomerUseCase>>());
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ReturnsCustomerDto()
    {
        Customer? captured = null;
        _customerRepo.Setup(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Callback<Customer, CancellationToken>((c, _) => captured = c)
            .ReturnsAsync(() => captured!);

        var request = new CreateCustomerRequest { Name = "Acme", Email = "acme@test.com" };
        var result = await _sut.ExecuteAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Acme");
        result.Value.Email.Should().Be("acme@test.com");
        captured.Should().NotBeNull();
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithAddress_SetsAddress()
    {
        Customer? captured = null;
        _customerRepo.Setup(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Callback<Customer, CancellationToken>((c, _) => captured = c)
            .ReturnsAsync(() => captured!);

        var request = new CreateCustomerRequest
        {
            Name = "Acme",
            Email = "a@b.com",
            Street = "1 rue X",
            City = "Paris",
            PostalCode = "75001",
            Country = "France"
        };
        var result = await _sut.ExecuteAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Street.Should().Be("1 rue X");
        result.Value.City.Should().Be("Paris");
    }

    [Fact]
    public async Task ExecuteAsync_InvalidEmail_ReturnsFailure()
    {
        var request = new CreateCustomerRequest { Name = "Acme", Email = "invalid" };
        var result = await _sut.ExecuteAsync(request);
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("VALIDATION");
    }
}
