using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Invoices;
using Hexagonal.Application.Ports.In;
using Hexagonal.Application.Ports.Out;
using Hexagonal.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Hexagonal.Application.UseCases.Invoices;

public class GenerateInvoiceUseCase : IGenerateInvoiceInputPort
{
    private readonly IOrderRepository _orderRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IInvoiceNumberGenerator _invoiceNumberGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GenerateInvoiceUseCase> _logger;

    public GenerateInvoiceUseCase(
        IOrderRepository orderRepository,
        IInvoiceRepository invoiceRepository,
        IInvoiceNumberGenerator invoiceNumberGenerator,
        IUnitOfWork unitOfWork,
        ILogger<GenerateInvoiceUseCase> logger)
    {
        _orderRepository = orderRepository;
        _invoiceRepository = invoiceRepository;
        _invoiceNumberGenerator = invoiceNumberGenerator;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<InvoiceDto>> ExecuteAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            return Result<InvoiceDto>.Failure($"Commande non trouvée : {orderId}", "ORDER_NOT_FOUND");

        if (order.Status != OrderStatus.Confirmed)
            return Result<InvoiceDto>.Failure("Seules les commandes confirmées peuvent être facturées.", "ORDER_NOT_CONFIRMED");

        var existing = await _invoiceRepository.GetByOrderIdAsync(orderId, cancellationToken);
        if (existing != null)
            return Result<InvoiceDto>.Failure("Une facture existe déjà pour cette commande.", "INVOICE_ALREADY_EXISTS");

        var invoiceNumber = await _invoiceNumberGenerator.GenerateAsync(cancellationToken);
        var invoice = Invoice.Create(invoiceNumber, order.Id, order.CustomerId, order.TotalAmount);
        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Facture générée : {InvoiceId} pour commande {OrderId}", invoice.Id, orderId);

        return Result<InvoiceDto>.Success(new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            OrderId = invoice.OrderId,
            CustomerId = invoice.CustomerId,
            TotalAmountEur = invoice.TotalAmount.Amount,
            Status = invoice.Status.ToString(),
            CreatedAtUtc = invoice.CreatedAtUtc
        });
    }
}
