using Hexagonal.API.Models;
using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Products;
using Hexagonal.Application.Ports.In;
using Microsoft.AspNetCore.Mvc;

namespace Hexagonal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly ICreateProductInputPort _createProduct;

    public ProductsController(ICreateProductInputPort createProduct)
    {
        _createProduct = createProduct;
    }

    /// <summary>Créer un produit.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _createProduct.ExecuteAsync(request, cancellationToken);
        if (!result.IsSuccess)
            return result.ErrorCode == "DUPLICATE_CODE"
                ? Conflict(new ErrorResponse { Code = result.ErrorCode!, Message = result.ErrorMessage })
                : BadRequest(new ErrorResponse { Code = result.ErrorCode ?? "ERROR", Message = result.ErrorMessage });
        return CreatedAtAction(nameof(Create), new { id = result.Value!.Id }, result.Value);
    }
}
