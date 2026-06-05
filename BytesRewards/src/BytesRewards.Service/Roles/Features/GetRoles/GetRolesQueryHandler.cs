using AppWeaver.Mediator.Interfaces;
using BytesRewards.Service.Infrastructure.Security.Keycloak;

namespace BytesRewards.Service.Roles.Features.GetRoles;

public sealed class GetRolesQueryHandler(
    IKeycloakAdminService keycloakAdminService)
    : IQueryHandler<
        GetRolesQuery,
        List<RoleResponse>>
{
    public async ValueTask<List<RoleResponse>> Handle(
    GetRolesQuery request,
    CancellationToken ct)
    {
        var token =
            await keycloakAdminService
                .GetAdminTokenAsync(ct);

        var roles =
            await keycloakAdminService
                .GetRolesAsync(token, ct);

        return roles
            .Where(x =>
                x != "offline_access" &&
                x != "uma_authorization")
            .Select(x => new RoleResponse
            {
                Name = x
            })
            .ToList();
    }
}