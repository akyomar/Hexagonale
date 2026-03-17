using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Customers;

namespace Hexagonal.Application.Ports.In;

/// <summary>
/// Port entrant : création d'un client.
/// </summary>
public interface ICreateCustomerInputPort
{
    Task<Result<CustomerDto>> ExecuteAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
}
