using FluentValidation;

namespace BytesRewards.Service.RewardsCatalog.Features.CreateRewardItem;

public sealed class CreateRewardItemValidator
    : AbstractValidator<CreateRewardItemCommand>
{
    public CreateRewardItemValidator()
    {
        RuleFor(x => x.ProductCode)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.RequiredBytes)
            .GreaterThan(0);
    }
}