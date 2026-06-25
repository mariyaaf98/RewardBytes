using FluentValidation;

namespace BytesRewards.Service.Designations.Features.DeleteDesignation;

public sealed class DeleteDesignationValidator
    : AbstractValidator<DeleteDesignationCommand>
{
    public DeleteDesignationValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Designation ID is required.");
    }
}
