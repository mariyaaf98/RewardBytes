using Microsoft.EntityFrameworkCore;
using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Appreciations.Domain;
using BytesRewards.Service.Infrastructure;
using BytesRewards.Service.Notifications.Services;

namespace BytesRewards.Service.Appreciations.Features.CreateAppreciation;

public sealed class CreateAppreciationCommandHandler(
    ApplicationDbContext context,
    NotificationService notifications)
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

        // ── Notifications ────────────────────────────────────────
        // Recipient: you received an appreciation
        notifications.Create(
            userId:  request.ToUserId,
            type:    "AppreciationReceived",
            title:   $"✨ {appreciation.FromUserName} appreciated you!",
            message: appreciation.Message);

        // Sender: confirmation that appreciation was sent
        notifications.Create(
            userId:  request.FromUserId,
            type:    "AppreciationSent",
            title:   $"✅ Appreciation sent to {appreciation.ToUserName}",
            message: $"Your appreciation was sent successfully.");

        // Broadcast to everyone else — peer recognition is public
        await notifications.CreateForAllUsersExceptAsync(
            excludeUserIds: [request.FromUserId, request.ToUserId],
            type:           "TeamAppreciation",
            title:          $"✨ {appreciation.FromUserName} appreciated {appreciation.ToUserName}",
            message:        appreciation.Message.Length > 120
                                ? appreciation.Message[..120] + "…"
                                : appreciation.Message,
            ct:             ct);

        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(appreciation.Id);
    }
}