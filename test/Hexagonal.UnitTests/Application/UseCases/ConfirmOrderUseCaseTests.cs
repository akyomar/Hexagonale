using FluentAssertions;
using Hexagonal.Application.Ports.Out;
using Hexagonal.Application.UseCases.Orders;
using Hexagonal.Domain.Entities;
using Hexagonal.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Hexagonal.UnitTests.Application.UseCases;

public class ConfirmOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _orderRepo;
    private readonly Mock<IUnitOfWork> _uow;
    private readonly ConfirmOrderUseCase _sut;

    public ConfirmOrderUseCaseTests()
    {
        _orderRepo = new Mock<IOrderRepository>();
        _uow = new Mock<IUnitOfWork>();
        _sut = new ConfirmOrderUseCase(_orderRepo.Object, _uow.Object, Mock.Of<ILogger<ConfirmOrderUseCase>>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenOrderExistsAndDraft_ConfirmsAndReturnsDto()
    {
        var order = Order.Create(Guid.NewGuid(), "ORD-001");
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "P1", "P", 1, Money.FromEur(10m)));
        _orderRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);

        var result = await _sut.ExecuteAsync(order.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be("Confirmed");
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOrderNotFound_ReturnsFailure()
    {
        _orderRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

        var result = await _sut.ExecuteAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("NOT_FOUND");
    }

    [Fact]
    public async Task ExecuteAsync_WhenOrderHasNoItems_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), "ORD-001");
        _orderRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);

        var result = await _sut.ExecuteAsync(order.Id);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("DOMAIN_RULE");
    }
}
