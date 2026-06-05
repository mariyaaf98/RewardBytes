using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.RewardCategories.Features.DeleteRewardCategory;

public sealed class DeleteRewardCategoryEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        DeleteRewardCategoryRequest,
        bool>
{
    public override void Configure()
    {
        Delete("/reward-categories/{id}");

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
            DeleteRewardCategoryRequest req,
            CancellationToken ct)
    {
        return await mediator.Send(
            new DeleteRewardCategoryCommand(
                req.Id),
            ct);
    }
}