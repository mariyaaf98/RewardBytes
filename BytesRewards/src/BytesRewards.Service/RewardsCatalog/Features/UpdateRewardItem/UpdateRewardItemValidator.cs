using FluentValidation;

namespace BytesRewards.Service.RewardsCatalog.Features.UpdateRewardItem;

public sealed class UpdateRewardItemValidator
    : AbstractValidator<UpdateRewardItemCommand>
{
    public UpdateRewardItemValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.ProductCode)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.RequiredBytes)
            .GreaterThan(0);
    }
}