using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.RewardsCatalog.Domain;

namespace BytesRewards.Service.RewardsCatalog.Features.CreateRewardItem;

public sealed class CreateRewardItemCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<
        CreateRewardItemCommand,
        Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        CreateRewardItemCommand request,
        CancellationToken ct)
    {
        var rewardItem = new RewardItem
        {
            Id = Guid.NewGuid(),

            ProductCode = request.ProductCode,

            Name = request.Name,

            Description = request.Description,

            RequiredBytes = request.RequiredBytes,

            IsActive = request.IsActive,

            ImageUrl = request.ImageUrl,

            CreatedAt = DateTime.UtcNow
        };

        context.RewardItems.Add(rewardItem);

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(
            rewardItem.Id);
    }
}