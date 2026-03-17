using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Invoices;

namespace Hexagonal.Application.Ports.In;

public interface IGenerateInvoiceInputPort
{
    Task<Result<InvoiceDto>> ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default);
}
