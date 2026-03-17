using FluentValidation;
using Hexagonal.Application.DTOs.Products;

namespace Hexagonal.Application.Validation;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Le code produit est requis.")
            .MaximumLength(50);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Le nom produit est requis.")
            .MaximumLength(200);

        RuleFor(x => x.UnitPriceEur)
            .GreaterThanOrEqualTo(0).WithMessage("Le prix unitaire ne peut pas être négatif.");
    }
}
