using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Orders;

namespace Hexagonal.Application.Ports.In;

public interface IGetOrderByIdInputPort
{
    Task<Result<OrderDto>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}
