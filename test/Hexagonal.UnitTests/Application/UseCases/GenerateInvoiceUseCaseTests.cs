using FluentAssertions;
using Hexagonal.Application.Ports.Out;
using Hexagonal.Application.UseCases.Invoices;
using Hexagonal.Domain.Entities;
using Hexagonal.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Hexagonal.UnitTests.Application.UseCases;

public class GenerateInvoiceUseCaseTests
{
    private readonly Mock<IOrderRepository> _orderRepo;
    private readonly Mock<IInvoiceRepository> _invoiceRepo;
    private readonly Mock<IInvoiceNumberGenerator> _numberGen;
    private readonly Mock<IUnitOfWork> _uow;
    private readonly GenerateInvoiceUseCase _sut;

    public GenerateInvoiceUseCaseTests()
    {
        _orderRepo = new Mock<IOrderRepository>();
        _invoiceRepo = new Mock<IInvoiceRepository>();
        _numberGen = new Mock<IInvoiceNumberGenerator>();
        _uow = new Mock<IUnitOfWork>();
        _sut = new GenerateInvoiceUseCase(
            _orderRepo.Object,
            _invoiceRepo.Object,
            _numberGen.Object,
            _uow.Object,
            Mock.Of<ILogger<GenerateInvoiceUseCase>>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenOrderConfirmed_GeneratesInvoice()
    {
        var order = Order.Create(Guid.NewGuid(), "ORD-001");
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "P1", "P", 1, Money.FromEur(100m)));
        order.Confirm();
        _orderRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);
        _invoiceRepo.Setup(r => r.GetByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Invoice?)null);
        _numberGen.Setup(g => g.GenerateAsync(It.IsAny<CancellationToken>())).ReturnsAsync("FACT-2024-0001");
        Invoice? captured = null;
        _invoiceRepo.Setup(r => r.AddAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>()))
            .Callback<Invoice, CancellationToken>((i, _) => captured = i)
            .ReturnsAsync(() => captured!);

        var result = await _sut.ExecuteAsync(order.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.InvoiceNumber.Should().Be("FACT-2024-0001");
        result.Value.TotalAmountEur.Should().Be(100m);
        captured.Should().NotBeNull();
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOrderNotFound_ReturnsFailure()
    {
        _orderRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

        var result = await _sut.ExecuteAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("ORDER_NOT_FOUND");
    }

    [Fact]
    public async Task ExecuteAsync_WhenOrderNotConfirmed_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), "ORD-001");
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "P1", "P", 1, Money.FromEur(10m)));
        _orderRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);

        var result = await _sut.ExecuteAsync(order.Id);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("ORDER_NOT_CONFIRMED");
    }

    [Fact]
    public async Task ExecuteAsync_WhenInvoiceAlreadyExists_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), "ORD-001");
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "P1", "P", 1, Money.FromEur(10m)));
        order.Confirm();
        var existingInvoice = Invoice.Create("FACT-1", order.Id, order.CustomerId, order.TotalAmount);
        _orderRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);
        _invoiceRepo.Setup(r => r.GetByOrderIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(existingInvoice);

        var result = await _sut.ExecuteAsync(order.Id);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("INVOICE_ALREADY_EXISTS");
    }
}
