using FluentValidation;

namespace BytesRewards.Service.Redemptions.Features.RedeemReward;

public sealed class RedeemRewardValidator
    : AbstractValidator<RedeemRewardCommand>
{
    public RedeemRewardValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.RewardItemId)
            .NotEmpty();
    }
}