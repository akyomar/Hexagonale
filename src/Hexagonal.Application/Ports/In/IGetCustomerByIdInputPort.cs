using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Customers;

namespace Hexagonal.Application.Ports.In;

public interface IGetCustomerByIdInputPort
{
    Task<Result<CustomerDto>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}
