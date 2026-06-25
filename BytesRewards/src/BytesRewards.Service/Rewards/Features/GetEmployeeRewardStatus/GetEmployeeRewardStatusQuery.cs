using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.Rewards.Features.GetEmployeeRewardStatus;

public sealed record GetEmployeeRewardStatusQuery
    : IQuery<EmployeeRewardStatusResponse>;
