using FluentValidation;
using Hexagonal.Application.DTOs.Orders;

namespace Hexagonal.Application.Validation;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Le client est requis.");

        RuleFor(x => x.OrderNumber)
            .NotEmpty().WithMessage("Le numéro de commande est requis.")
            .MaximumLength(50);

        RuleFor(x => x.Lines)
            .NotEmpty().WithMessage("La commande doit contenir au moins une ligne.");

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.ProductId).NotEmpty();
            line.RuleFor(l => l.Quantity).GreaterThan(0).WithMessage("La quantité doit être positive.");
        });
    }
}
