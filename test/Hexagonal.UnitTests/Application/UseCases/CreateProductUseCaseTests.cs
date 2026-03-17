using FluentAssertions;
using Hexagonal.Application.DTOs.Products;
using Hexagonal.Application.Ports.Out;
using Hexagonal.Application.UseCases.Products;
using Hexagonal.Domain.Entities;
using Hexagonal.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Hexagonal.UnitTests.Application.UseCases;

public class CreateProductUseCaseTests
{
    private readonly Mock<IProductRepository> _productRepo;
    private readonly Mock<IUnitOfWork> _uow;
    private readonly CreateProductUseCase _sut;

    public CreateProductUseCaseTests()
    {
        _productRepo = new Mock<IProductRepository>();
        _uow = new Mock<IUnitOfWork>();
        _sut = new CreateProductUseCase(_productRepo.Object, _uow.Object, Mock.Of<ILogger<CreateProductUseCase>>());
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ReturnsProductDto()
    {
        Product? captured = null;
        _productRepo.Setup(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);
        _productRepo.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((p, _) => captured = p)
            .ReturnsAsync(() => captured!);

        var request = new CreateProductRequest { Code = "P1", Name = "Product 1", UnitPriceEur = 10m };
        var result = await _sut.ExecuteAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Code.Should().Be("P1");
        result.Value.Name.Should().Be("Product 1");
        result.Value.UnitPriceEur.Should().Be(10m);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_DuplicateCode_ReturnsFailure()
    {
        var existing = Product.Create("P1", "Existing", Money.FromEur(1));
        _productRepo.Setup(r => r.GetByCodeAsync("P1", It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        var request = new CreateProductRequest { Code = "P1", Name = "New", UnitPriceEur = 5m };
        var result = await _sut.ExecuteAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("DUPLICATE_CODE");
    }

    [Fact]
    public async Task ExecuteAsync_InvalidPrice_ReturnsFailure()
    {
        _productRepo.Setup(r => r.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var request = new CreateProductRequest { Code = "P1", Name = "P", UnitPriceEur = -1m };
        var result = await _sut.ExecuteAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("VALIDATION");
    }
}
