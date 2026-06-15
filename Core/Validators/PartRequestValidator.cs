using Domain.Dtos;
using FluentValidation;

namespace Core.Validators;

public class PartRequestValidator : AbstractValidator<PartRequest>
{
    public PartRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Part name is required.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Part description is required.");
    }
}
