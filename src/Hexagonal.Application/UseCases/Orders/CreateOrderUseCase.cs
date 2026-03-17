using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Orders;
using Hexagonal.Application.Ports.In;
using Hexagonal.Application.Ports.Out;
using Hexagonal.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Hexagonal.Application.UseCases.Orders;

public class CreateOrderUseCase : ICreateOrderInputPort
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateOrderUseCase> _logger;

    public CreateOrderUseCase(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateOrderUseCase> logger)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<OrderDto>> ExecuteAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
            return Result<OrderDto>.Failure($"Client non trouvé : {request.CustomerId}", "CUSTOMER_NOT_FOUND");

        var existingOrder = await _orderRepository.GetByOrderNumberAsync(request.OrderNumber, cancellationToken);
        if (existingOrder != null)
            return Result<OrderDto>.Failure($"Une commande avec le numéro '{request.OrderNumber}' existe déjà.", "DUPLICATE_ORDER_NUMBER");

        var productIds = request.Lines.Select(l => l.ProductId).Distinct().ToList();
        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
        var productMap = products.ToDictionary(p => p.Id);

        if (productMap.Count != productIds.Count)
        {
            var missing = productIds.Except(productMap.Keys).First();
            return Result<OrderDto>.Failure($"Produit non trouvé : {missing}", "PRODUCT_NOT_FOUND");
        }

        try
        {
            var order = Order.Create(request.CustomerId, request.OrderNumber);

            foreach (var line in request.Lines)
            {
                var product = productMap[line.ProductId];
                if (!product.IsActive)
                    return Result<OrderDto>.Failure($"Le produit '{product.Code}' n'est plus actif.", "PRODUCT_INACTIVE");

                var item = OrderItem.Create(order.Id, product.Id, product.Code, product.Name, line.Quantity, product.UnitPrice);
                order.AddItem(item);
            }

            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Commande créée : {OrderId} - {OrderNumber}", order.Id, order.OrderNumber);

            return Result<OrderDto>.Success(MapToDto(order));
        }
        catch (ArgumentException ex)
        {
            return Result<OrderDto>.Failure(ex.Message, "VALIDATION");
        }
        catch (InvalidOperationException ex)
        {
            return Result<OrderDto>.Failure(ex.Message, "DOMAIN_RULE");
        }
    }

    private static OrderDto MapToDto(Order o) => new()
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
