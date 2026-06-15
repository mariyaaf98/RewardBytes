using FluentValidation;

namespace BytesRewards.Service.RewardCategories.Features.CreateRewardCategory;

public sealed class CreateRewardCategoryValidator
    : AbstractValidator<CreateRewardCategoryCommand>
{
    public CreateRewardCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(100)
            .WithMessage("Category name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Bytes)
            .NotEmpty()
            .WithMessage("Bytes value is required.")
            .GreaterThan(0)
            .WithMessage("Bytes value must be greater than 0.");
    }
}
