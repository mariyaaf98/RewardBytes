using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.RewardCategories.Features.UpdateRewardCategory;

public sealed class UpdateRewardCategoryEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        UpdateRewardCategoryRequest,
        bool>
{
    public override void Configure()
    {
        Put("/reward-categories/{id}");

        Roles("manager");

        Options(option =>
            option.WithTags("05 - Reward Categories"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy()
        => new()
        {
            SecurityLevel = SecurityLevel.Internal,
            CachePolicy = CachePolicy.NoStore
        };

    protected override async Task<Result<bool>>
        ExecuteAsync(
            UpdateRewardCategoryRequest req,
            CancellationToken ct)
    {
        return await mediator.Send(
            new UpdateRewardCategoryCommand(
                req.Id,
                req.Name,
                req.Description,
                req.Bytes),
            ct);
    }
}