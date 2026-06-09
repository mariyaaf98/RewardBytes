using FluentValidation;

namespace BytesRewards.Service.Features.Users.CreateUser;

public class CreateUserValidator
    : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^\d{10}$")
            .WithMessage("Phone number must contain exactly 10 digits");

        RuleFor(x => x.Designation)
            .NotEmpty()
            .WithMessage("Designation is required");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("Role is required");

        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithMessage("Department is required");
    }
}