using FluentValidation;

namespace BytesRewards.Service.Users.Features.ToggleUserStatus;

public sealed class ToggleUserStatusValidator
    : AbstractValidator<ToggleUserStatusCommand>
{
    public ToggleUserStatusValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}
