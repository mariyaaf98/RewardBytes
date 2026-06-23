using FluentValidation;

namespace BytesRewards.Service.Redemptions.Features.UpdateRedemptionStatus;

public sealed class UpdateRedemptionStatusValidator
    : AbstractValidator<UpdateRedemptionStatusCommand>
{
    public UpdateRedemptionStatusValidator()
    {
        RuleFor(x => x.RedemptionId)
            .NotEmpty();

        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(x =>
                x == "Pending" ||
                x == "Approved" ||
                x == "Rejected" ||
                x == "Delivered")
            .WithMessage(
                "Invalid status");
    }
}