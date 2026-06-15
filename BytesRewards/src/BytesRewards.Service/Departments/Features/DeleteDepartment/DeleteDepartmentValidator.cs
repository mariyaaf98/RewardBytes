using FluentValidation;

namespace BytesRewards.Service.Departments.Features.DeleteDepartment;

public sealed class DeleteDepartmentValidator
    : AbstractValidator<DeleteDepartmentCommand>
{
    public DeleteDepartmentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Department ID is required.");
    }
}
