using AppWeaver.FastEndpoint;
using AppWeaver.Mediator;
using AppWeaver.Results;
using AppWeaver.Web.Security;

namespace BytesRewards.Service.RewardCategories.Features.CreateRewardCategory;

public sealed class CreateRewardCategoryEndpoint(
    IMediator mediator)
    : SecureFastEndpoint<
        CreateRewardCategoryRequest,
        Guid>
{
    public override void Configure()
    {
        Post("/reward-categories");

        Roles("admin", "manager");

        Options(option =>
            option.WithTags("05 - Reward Categories"));
    }

    protected override SecurityCachePolicy GetSecurityCachePolicy()
        => new()
        {
            SecurityLevel = SecurityLevel.Internal,
            CachePolicy = CachePolicy.NoStore
        };

    protected override async Task<Result<Guid>>
        ExecuteAsync(
            CreateRewardCategoryRequest req,
            CancellationToken ct)
    {
        return await mediator.Send(
            new CreateRewardCategoryCommand(
                req.Name,
                req.Description,
                req.Bytes),
            ct);
    }
}