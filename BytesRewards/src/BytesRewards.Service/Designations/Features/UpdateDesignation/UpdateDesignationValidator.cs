using FluentValidation;

namespace BytesRewards.Service.Designations.Features.UpdateDesignation;

public sealed class UpdateDesignationValidator
    : AbstractValidator<UpdateDesignationCommand>
{
    public UpdateDesignationValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Designation ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Designation name is required.")
            .MaximumLength(200)
            .WithMessage("Designation name must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");
    }
}
