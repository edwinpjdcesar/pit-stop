using Domain.Dtos;
using FluentValidation;

namespace Core.Validators;

public class MaintenancePartRequestValidator : AbstractValidator<MaintenancePartRequest>
{
    public MaintenancePartRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity of maintenance part must be greater than zero.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price of maintenance part must be non-negative.");
    }
}
