using Hexagonal.Application.Common;
using Hexagonal.Application.DTOs.Products;
using Hexagonal.Application.Ports.In;
using Hexagonal.Application.Ports.Out;
using Hexagonal.Domain.Entities;
using Hexagonal.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Hexagonal.Application.UseCases.Products;

public class CreateProductUseCase : ICreateProductInputPort
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateProductUseCase> _logger;

    public CreateProductUseCase(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateProductUseCase> logger)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ProductDto>> ExecuteAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var existing = await _productRepository.GetByCodeAsync(request.Code, cancellationToken);
            if (existing != null)
                return Result<ProductDto>.Failure($"Un produit avec le code '{request.Code}' existe déjà.", "DUPLICATE_CODE");

            var unitPrice = Money.FromEur(request.UnitPriceEur);
            var product = Product.Create(request.Code, request.Name, unitPrice, request.Description);
            await _productRepository.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Produit créé : {ProductId} - {Code}", product.Id, product.Code);

            return Result<ProductDto>.Success(new ProductDto
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
                Description = product.Description,
                UnitPriceEur = product.UnitPrice.Amount,
                IsActive = product.IsActive,
                CreatedAtUtc = product.CreatedAtUtc
            });
        }
        catch (ArgumentException ex)
        {
            return Result<ProductDto>.Failure(ex.Message, "VALIDATION");
        }
    }
}
