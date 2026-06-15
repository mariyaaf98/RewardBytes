using FluentValidation;

namespace BytesRewards.Service.Appreciations.Features.CreateAppreciation;

public sealed class CreateAppreciationValidator
    : AbstractValidator<CreateAppreciationCommand>
{
    public CreateAppreciationValidator()
    {
        RuleFor(x => x.ToUserId)
            .NotEmpty()
            .WithMessage("Recipient is required.");

        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Message is required.")
            .MinimumLength(10)
            .WithMessage("Message must be at least 10 characters.")
            .MaximumLength(1000)
            .WithMessage("Message must not exceed 1000 characters.");

        RuleFor(x => x.FromUserId)
            .NotEmpty()
            .WithMessage("Sender is required.")
            .NotEqual(x => x.ToUserId)
            .WithMessage("You cannot send a recognition to yourself.");
    }
}
