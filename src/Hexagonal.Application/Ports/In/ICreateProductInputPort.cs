using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Products;

namespace Hexagonal.Application.Ports.In;

public interface ICreateProductInputPort
{
    Task<Result<ProductDto>> ExecuteAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
}
