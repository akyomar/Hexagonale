using Hexagonal.API.Models;
using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Customers;
using Hexagonal.Application.Ports.In;
using Microsoft.AspNetCore.Mvc;

namespace Hexagonal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CustomersController : ControllerBase
{
    private readonly ICreateCustomerInputPort _createCustomer;
    private readonly IGetCustomerByIdInputPort _getCustomerById;

    public CustomersController(
        ICreateCustomerInputPort createCustomer,
        IGetCustomerByIdInputPort getCustomerById)
    {
        _createCustomer = createCustomer;
        _getCustomerById = getCustomerById;
    }

    /// <summary>Créer un client.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await _createCustomer.ExecuteAsync(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : ToErrorResult(result);
    }

    /// <summary>Récupérer un client par id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _getCustomerById.ExecuteAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : ToErrorResult(result);
    }

    private IActionResult ToErrorResult(Result result)
    {
        return (result.ErrorCode) switch
        {
            "NOT_FOUND" => NotFound(new ErrorResponse { Code = result.ErrorCode!, Message = result.ErrorMessage }),
            _ => BadRequest(new ErrorResponse { Code = result.ErrorCode ?? "ERROR", Message = result.ErrorMessage })
        };
    }

    private IActionResult ToErrorResult<T>(Result<T> result) => ToErrorResult((Result)result);
}
