using Domain.Dtos;
using FluentValidation;

namespace Core.Validators;

public class VehicleRequestValidator : AbstractValidator<VehicleRequest>
{
    public VehicleRequestValidator()
    {
        RuleFor(x => x.MakeId)
            .GreaterThan(0).WithMessage("Make of vehicle must be valid.");

        RuleFor(x => x.ModelId)
            .GreaterThan(0).WithMessage("Model of vehicle must be valid.");

        RuleFor(x => x.Year)
            .InclusiveBetween(1886, DateTime.UtcNow.Year + 1)
            .WithMessage($"Year of vehicle must be between 1886 and {DateTime.UtcNow.Year + 1}.");

        RuleFor(x => x.PurchasePrice)
            .GreaterThanOrEqualTo(0).WithMessage("Purhcase price must be non-negative.")
            .When(x => x.PurchasePrice.HasValue);

        RuleFor(x => x.MileageAtPurchase)
            .GreaterThanOrEqualTo(0).WithMessage("Mileage at purchase must be non-negative.")
            .When(x => x.MileageAtPurchase.HasValue);

        RuleFor(x => x.Mileage)
            .GreaterThanOrEqualTo(0).WithMessage("Current mileage of vehicle must be non-negative.")
            .When(x => x.Mileage.HasValue);
    }
}
