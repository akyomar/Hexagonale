using Hexagonal.Application.Ports.Out;

namespace Hexagonal.Infrastructure.Services;

public class InvoiceNumberGeneratorService : IInvoiceNumberGenerator
{
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceNumberGeneratorService(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<string> GenerateAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.UtcNow.Year;
        var seq = await _invoiceRepository.GetNextSequenceForYearAsync(year, cancellationToken);
        return $"FACT-{year}-{seq:D4}";
    }
}
