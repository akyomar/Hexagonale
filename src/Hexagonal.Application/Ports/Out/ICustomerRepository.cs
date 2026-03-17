using Hexagonal.Domain.Entities;

namespace Hexagonal.Application.Ports.Out;

/// <summary>
/// Port sortant : persistance des clients.
/// </summary>
public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer> AddAsync(Customer customer, CancellationToken cancellationToken = default);
}
