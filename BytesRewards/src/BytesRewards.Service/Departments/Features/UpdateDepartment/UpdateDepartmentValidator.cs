using FluentValidation;

namespace BytesRewards.Service.Departments.Features.UpdateDepartment;

public sealed class UpdateDepartmentValidator
    : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Department ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Department name is required.")
            .MaximumLength(100)
            .WithMessage("Department name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");
    }
}
