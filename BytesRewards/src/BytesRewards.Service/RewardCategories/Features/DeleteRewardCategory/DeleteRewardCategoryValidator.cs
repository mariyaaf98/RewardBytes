using FluentValidation;

namespace BytesRewards.Service.RewardCategories.Features.DeleteRewardCategory;

public sealed class DeleteRewardCategoryValidator
    : AbstractValidator<DeleteRewardCategoryCommand>
{
    public DeleteRewardCategoryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Category ID is required.");
    }
}
