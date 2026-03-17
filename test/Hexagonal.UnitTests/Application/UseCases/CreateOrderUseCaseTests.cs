using Hexagonal.Application.DTOs.Orders;
using Hexagonal.Application.Ports.Out;
using Hexagonal.Application.UseCases.Orders;
using Hexagonal.Domain.Entities;
using Hexagonal.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace Hexagonal.UnitTests.Application.UseCases;

public class CreateOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _orderRepo;
    private readonly Mock<ICustomerRepository> _customerRepo;
    private readonly Mock<IProductRepository> _productRepo;
    private readonly Mock<IUnitOfWork> _uow;
    private readonly CreateOrderUseCase _sut;

    public CreateOrderUseCaseTests()
    {
        _orderRepo = new Mock<IOrderRepository>();
        _customerRepo = new Mock<ICustomerRepository>();
        _productRepo = new Mock<IProductRepository>();
        _uow = new Mock<IUnitOfWork>();
        var logger = new Mock<ILogger<CreateOrderUseCase>>();
        _sut = new CreateOrderUseCase(_orderRepo.Object, _customerRepo.Object, _productRepo.Object, _uow.Object, logger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_CustomerNotFound_ReturnsFailure()
    {
        _customerRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Customer?)null);
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid(),
            OrderNumber = "ORD-001",
            Lines = new List<OrderLineRequest> { new() { ProductId = Guid.NewGuid(), Quantity = 1 } }
        };

        var result = await _sut.ExecuteAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("CUSTOMER_NOT_FOUND");
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_CreatesOrder()
    {
        var customerId = Guid.NewGuid();
        var customer = Customer.Create("Test", Email.Create("t@t.com"));
        var product = Product.Create("P1", "Product 1", Money.FromEur(10m));
        _customerRepo.Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        _productRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { product });
        _orderRepo.Setup(r => r.GetByOrderNumberAsync("ORD-001", It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);
        Order? capturedOrder = null;
        _orderRepo.Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Callback<Order, CancellationToken>((o, _) => capturedOrder = o)
            .ReturnsAsync(() => capturedOrder!);

        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            OrderNumber = "ORD-001",
            Lines = new List<OrderLineRequest> { new() { ProductId = product.Id, Quantity = 2 } }
        };

        var result = await _sut.ExecuteAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value!.OrderNumber.Should().Be("ORD-001");
        result.Value.TotalAmountEur.Should().Be(20m);
        result.Value.Lines.Should().HaveCount(1);
        capturedOrder.Should().NotBeNull();
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_DuplicateOrderNumber_ReturnsFailure()
    {
        var customer = Customer.Create("Test", Email.Create("t@t.com"));
        var existingOrder = Order.Create(customer.Id, "ORD-001");
        _customerRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        _orderRepo.Setup(r => r.GetByOrderNumberAsync("ORD-001", It.IsAny<CancellationToken>())).ReturnsAsync(existingOrder);

        var request = new CreateOrderRequest
        {
            CustomerId = customer.Id,
            OrderNumber = "ORD-001",
            Lines = new List<OrderLineRequest> { new() { ProductId = Guid.NewGuid(), Quantity = 1 } }
        };
        var result = await _sut.ExecuteAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("DUPLICATE_ORDER_NUMBER");
    }

    [Fact]
    public async Task ExecuteAsync_ProductNotFound_ReturnsFailure()
    {
        var customer = Customer.Create("Test", Email.Create("t@t.com"));
        var productId = Guid.NewGuid();
        _customerRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        _productRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>()); // aucun produit retourné
        _orderRepo.Setup(r => r.GetByOrderNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

        var request = new CreateOrderRequest
        {
            CustomerId = customer.Id,
            OrderNumber = "ORD-001",
            Lines = new List<OrderLineRequest> { new() { ProductId = productId, Quantity = 1 } }
        };
        var result = await _sut.ExecuteAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("PRODUCT_NOT_FOUND");
    }

    [Fact]
    public async Task ExecuteAsync_ProductInactive_ReturnsFailure()
    {
        var customer = Customer.Create("Test", Email.Create("t@t.com"));
        var product = Product.Create("P1", "Product", Money.FromEur(10m));
        product.Deactivate();
        _customerRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        _productRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product> { product });
        _orderRepo.Setup(r => r.GetByOrderNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

        var request = new CreateOrderRequest
        {
            CustomerId = customer.Id,
            OrderNumber = "ORD-001",
            Lines = new List<OrderLineRequest> { new() { ProductId = product.Id, Quantity = 1 } }
        };
        var result = await _sut.ExecuteAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("PRODUCT_INACTIVE");
    }
}
