using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Orders;

namespace Hexagonal.Application.Ports.In;

public interface ICreateOrderInputPort
{
    Task<Result<OrderDto>> ExecuteAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
}
