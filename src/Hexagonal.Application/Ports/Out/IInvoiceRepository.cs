using Hexagonal.Domain.Entities;

namespace Hexagonal.Application.Ports.Out;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<int> GetNextSequenceForYearAsync(int year, CancellationToken cancellationToken = default);
    Task<Invoice> AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
}
