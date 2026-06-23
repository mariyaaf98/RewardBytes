using Microsoft.EntityFrameworkCore;
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
        if (request.FromUserId == request.ToUserId)
            throw new Exception("You cannot appreciate yourself.");

        var fromUser = await context.Users
            .Where(x => x.Id == request.FromUserId)
            .Select(x => new { FullName = x.FirstName + " " + x.LastName })
            .FirstOrDefaultAsync(ct);

        var toUser = await context.Users
            .Where(x => x.Id == request.ToUserId)
            .Select(x => new { FullName = x.FirstName + " " + x.LastName })
            .FirstOrDefaultAsync(ct);

        var appreciation = new Appreciation
        {
            Id           = Guid.NewGuid(),
            FromUserId   = request.FromUserId,
            ToUserId     = request.ToUserId,
            Message      = request.Message,
            FromUserName = fromUser?.FullName ?? string.Empty,
            ToUserName   = toUser?.FullName   ?? string.Empty,
            CreatedAt    = DateTime.UtcNow
        };

        context.Appreciations.Add(appreciation);

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(appreciation.Id);
    }
}