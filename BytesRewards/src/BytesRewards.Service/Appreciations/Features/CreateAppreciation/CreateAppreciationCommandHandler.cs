using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

using BytesRewards.Service.Appreciations.Domain;
using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Appreciations.Features.CreateAppreciation;

public sealed class CreateAppreciationCommandHandler(
    ApplicationDbContext context)
    : ICommandHandler<CreateAppreciationCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        CreateAppreciationCommand request,
        CancellationToken ct)
    {
        var appreciation = new Appreciation
        {
            Id = Guid.NewGuid(),

            FromUserId = request.FromUserId,

            ToUserId = request.ToUserId,

            Message = request.Message,

            CreatedAt = DateTime.UtcNow
        };

        context.Appreciations.Add(appreciation);

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(appreciation.Id);
    }
}