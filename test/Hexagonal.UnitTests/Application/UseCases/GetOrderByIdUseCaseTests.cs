using FluentAssertions;
using Hexagonal.Application.Ports.Out;
using Hexagonal.Application.UseCases.Orders;
using Hexagonal.Domain.Entities;
using Hexagonal.Domain.ValueObjects;
using Moq;
using Xunit;

namespace Hexagonal.UnitTests.Application.UseCases;

public class GetOrderByIdUseCaseTests
{
    private readonly Mock<IOrderRepository> _orderRepo;
    private readonly GetOrderByIdUseCase _sut;

    public GetOrderByIdUseCaseTests()
    {
        _orderRepo = new Mock<IOrderRepository>();
        _sut = new GetOrderByIdUseCase(_orderRepo.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOrderExists_ReturnsOrderDto()
    {
        var order = Order.Create(Guid.NewGuid(), "ORD-001");
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "P1", "Product", 2, Money.FromEur(10m)));
        _orderRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(order);

        var result = await _sut.ExecuteAsync(order.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value!.OrderNumber.Should().Be("ORD-001");
        result.Value.TotalAmountEur.Should().Be(20m);
        result.Value.Lines.Should().HaveCount(1);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOrderNotFound_ReturnsFailure()
    {
        _orderRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Order?)null);

        var result = await _sut.ExecuteAsync(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("NOT_FOUND");
    }
}
