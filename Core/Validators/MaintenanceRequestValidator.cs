using Domain.Dtos;
using FluentValidation;

namespace Core.Validators;

public class MaintenanceRequestValidator : AbstractValidator<MaintenanceRequest>
{
    public MaintenanceRequestValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Maintenance description is required.");

        RuleFor(x => x.Mileage)
            .GreaterThanOrEqualTo(0).WithMessage("Mileage at maintenance must be non-negative.");

        RuleFor(x => x.ServiceDate)
            .NotEmpty().WithMessage("Maintenance service date is required.");
    }
}
