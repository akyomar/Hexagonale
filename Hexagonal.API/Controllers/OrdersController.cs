using Hexagonal.API.Models;
using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Orders;
using Hexagonal.Application.Ports.In;
using Microsoft.AspNetCore.Mvc;

namespace Hexagonal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly ICreateOrderInputPort _createOrder;
    private readonly IGetOrderByIdInputPort _getOrderById;
    private readonly IConfirmOrderInputPort _confirmOrder;

    public OrdersController(
        ICreateOrderInputPort createOrder,
        IGetOrderByIdInputPort getOrderById,
        IConfirmOrderInputPort confirmOrder)
    {
        _createOrder = createOrder;
        _getOrderById = getOrderById;
        _confirmOrder = confirmOrder;
    }

    /// <summary>Créer une commande.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _createOrder.ExecuteAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode switch
            {
                "CUSTOMER_NOT_FOUND" or "PRODUCT_NOT_FOUND" => NotFound(new ErrorResponse { Code = result.ErrorCode!, Message = result.ErrorMessage }),
                "DUPLICATE_ORDER_NUMBER" => Conflict(new ErrorResponse { Code = result.ErrorCode!, Message = result.ErrorMessage }),
                _ => BadRequest(new ErrorResponse { Code = result.ErrorCode ?? "ERROR", Message = result.ErrorMessage })
            };
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>Récupérer une commande par id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getOrderById.ExecuteAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new ErrorResponse { Code = result.ErrorCode!, Message = result.ErrorMessage });
    }

    /// <summary>Confirmer une commande (brouillon → confirmée).</summary>
    [HttpPost("{id:guid}/confirm")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        var result = await _confirmOrder.ExecuteAsync(id, cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "NOT_FOUND"
                ? NotFound(new ErrorResponse { Code = result.ErrorCode!, Message = result.ErrorMessage })
                : BadRequest(new ErrorResponse { Code = result.ErrorCode!, Message = result.ErrorMessage });
        return Ok(result.Value);
    }
}
