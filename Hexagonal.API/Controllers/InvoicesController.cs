using Hexagonal.API.Models;
using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Invoices;
using Hexagonal.Application.Ports.In;
using Microsoft.AspNetCore.Mvc;

namespace Hexagonal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InvoicesController : ControllerBase
{
    private readonly IGenerateInvoiceInputPort _generateInvoice;

    public InvoicesController(IGenerateInvoiceInputPort generateInvoice)
    {
        _generateInvoice = generateInvoice;
    }

    /// <summary>Générer une facture pour une commande confirmée.</summary>
    [HttpPost("generate/{orderId:guid}")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Generate(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _generateInvoice.ExecuteAsync(orderId, cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode switch
            {
                "ORDER_NOT_FOUND" => NotFound(new ErrorResponse { Code = result.ErrorCode!, Message = result.ErrorMessage }),
                "ORDER_NOT_CONFIRMED" or "INVOICE_ALREADY_EXISTS" => BadRequest(new ErrorResponse { Code = result.ErrorCode!, Message = result.ErrorMessage }),
                _ => BadRequest(new ErrorResponse { Code = result.ErrorCode ?? "ERROR", Message = result.ErrorMessage })
            };
        return CreatedAtAction(nameof(Generate), new { id = result.Value!.Id }, result.Value);
    }
}
