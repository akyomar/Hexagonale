using Hexagonal.Application.Ports.Out;
using Hexagonal.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hexagonal.Infrastructure.Persistence.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public InvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Invoices.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<Invoice?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default) =>
        await _context.Invoices.FirstOrDefaultAsync(i => i.OrderId == orderId, cancellationToken);

    public async Task<int> GetNextSequenceForYearAsync(int year, CancellationToken cancellationToken = default)
    {
        var prefix = $"FACT-{year}";
        var last = await _context.Invoices
            .Where(i => i.InvoiceNumber.StartsWith(prefix))
            .OrderByDescending(i => i.InvoiceNumber)
            .Select(i => i.InvoiceNumber)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrEmpty(last))
            return 1;
        var parts = last.Split('-');
        return parts.Length >= 3 && int.TryParse(parts[^1], out var seq) ? seq + 1 : 1;
    }

    public async Task<Invoice> AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        await _context.Invoices.AddAsync(invoice, cancellationToken);
        return invoice;
    }
}
