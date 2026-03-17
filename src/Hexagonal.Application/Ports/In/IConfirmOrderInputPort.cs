using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Orders;

namespace Hexagonal.Application.Ports.In;

public interface IConfirmOrderInputPort
{
    Task<Result<OrderDto>> ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default);
}
