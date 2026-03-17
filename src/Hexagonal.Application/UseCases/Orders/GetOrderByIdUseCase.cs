using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Orders;
using Hexagonal.Application.Ports.In;
using Hexagonal.Application.Ports.Out;
using Microsoft.Extensions.Logging;

namespace Hexagonal.Application.UseCases.Orders;

public class GetOrderByIdUseCase : IGetOrderByIdInputPort
{
    private readonly IOrderRepository _orderRepository;
    public GetOrderByIdUseCase(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderDto>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null)
            return Result<OrderDto>.Failure($"Commande non trouvée : {id}", "NOT_FOUND");

        return Result<OrderDto>.Success(new OrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            TotalAmountEur = order.TotalAmount.Amount,
            CreatedAtUtc = order.CreatedAtUtc,
            Lines = order.Items.Select(i => new OrderLineDto
            {
                ProductId = i.ProductId,
                ProductCode = i.ProductCode,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPriceEur = i.UnitPrice.Amount,
                LineTotalEur = i.LineTotal.Amount
            }).ToList()
        });
    }
}
