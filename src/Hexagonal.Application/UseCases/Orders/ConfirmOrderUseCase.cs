using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Orders;
using Hexagonal.Application.Ports.In;
using Hexagonal.Application.Ports.Out;
using Microsoft.Extensions.Logging;

namespace Hexagonal.Application.UseCases.Orders;

public class ConfirmOrderUseCase : IConfirmOrderInputPort
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ConfirmOrderUseCase> _logger;

    public ConfirmOrderUseCase(IOrderRepository orderRepository, IUnitOfWork unitOfWork, ILogger<ConfirmOrderUseCase> logger)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<OrderDto>> ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            return Result<OrderDto>.Failure($"Commande non trouvée : {orderId}", "NOT_FOUND");

        try
        {
            order.Confirm();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Commande confirmée : {OrderId}", orderId);
            return Result<OrderDto>.Success(MapToDto(order));
        }
        catch (InvalidOperationException ex)
        {
            return Result<OrderDto>.Failure(ex.Message, "DOMAIN_RULE");
        }
    }

    private static OrderDto MapToDto(Domain.Entities.Order o) => new()
    {
        Id = o.Id,
        CustomerId = o.CustomerId,
        OrderNumber = o.OrderNumber,
        Status = o.Status.ToString(),
        TotalAmountEur = o.TotalAmount.Amount,
        CreatedAtUtc = o.CreatedAtUtc,
        Lines = o.Items.Select(i => new OrderLineDto
        {
            ProductId = i.ProductId,
            ProductCode = i.ProductCode,
            ProductName = i.ProductName,
            Quantity = i.Quantity,
            UnitPriceEur = i.UnitPrice.Amount,
            LineTotalEur = i.LineTotal.Amount
        }).ToList()
    };
}
