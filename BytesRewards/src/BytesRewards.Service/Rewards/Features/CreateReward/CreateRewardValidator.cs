using FluentValidation;

namespace BytesRewards.Service.Rewards.Features.CreateReward;

public sealed class CreateRewardValidator
    : AbstractValidator<CreateRewardCommand>
{
    public CreateRewardValidator()
    {
        RuleFor(x => x.FromUserId)
            .NotEmpty()
            .WithMessage("Manager ID is required.");

        RuleFor(x => x.ToUserId)
            .NotEmpty()
            .WithMessage("Recipient employee is required.")
            .NotEqual(x => x.FromUserId)
            .WithMessage("You cannot assign a reward to yourself.");

        RuleFor(x => x.RewardCategoryId)
            .NotEmpty()
            .WithMessage("Reward category is required.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required.")
            .MinimumLength(10)
            .WithMessage("Reason must be at least 10 characters.")
            .MaximumLength(500)
            .WithMessage("Reason must not exceed 500 characters.");
    }
}
