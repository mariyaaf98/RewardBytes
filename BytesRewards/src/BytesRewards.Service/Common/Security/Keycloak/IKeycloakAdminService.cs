namespace BytesRewards.Service.Infrastructure.Security.Keycloak;

public interface IKeycloakAdminService
{
    Task<string> GetAdminTokenAsync(
        CancellationToken ct);

    Task<string> CreateUserAsync(
        string token,
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken ct);

    Task AssignRoleAsync(
        string token,
        string userId,
        string roleName,
        CancellationToken ct);

    Task<string> GetUserRoleAsync(
        string token,
        string userId,
        CancellationToken ct);

    Task UpdateUserAsync(
        string token,
        string userId,
        string firstName,
        string lastName,
        string email,
        CancellationToken ct);

    Task RemoveRoleAsync(
        string token,
        string userId,
        string roleName,
        CancellationToken ct);

    Task UpdateUserRoleAsync(
        string token,
        string userId,
        string roleName,
        CancellationToken ct);


    Task DisableUserAsync(
        string token,
        string userId,
        CancellationToken ct);

    Task EnableUserAsync(
        string token,
        string userId,
        CancellationToken ct);

    Task<List<string>> GetRolesAsync(
        string token,
        CancellationToken ct);
}