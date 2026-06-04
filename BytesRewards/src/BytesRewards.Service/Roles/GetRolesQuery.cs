using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.Roles.Features.GetRoles;

public sealed record GetRolesQuery
    : IQuery<List<RoleResponse>>;