using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;

namespace BytesRewards.Service.Redemptions.Features.UpdateRedemptionStatus;

public sealed record UpdateRedemptionStatusCommand(
    Guid RedemptionId,
    string Status)
    : ICommand<Result<Guid>>;