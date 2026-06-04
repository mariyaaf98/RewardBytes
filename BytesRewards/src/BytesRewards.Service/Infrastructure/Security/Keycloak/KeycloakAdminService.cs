using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using System.Linq;
using BytesRewards.Service.Infrastructure.Security.Keycloak.Models;

namespace BytesRewards.Service.Infrastructure.Security.Keycloak;

public class KeycloakAdminService(
    HttpClient httpClient,
    IOptions<KeycloakAdminOptions> options)
    : IKeycloakAdminService
{
    private readonly KeycloakAdminOptions _options =
        options.Value;

    public async Task<string> GetAdminTokenAsync(
        CancellationToken ct)
    {
        var formData =
            new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret
            };

        var response =
            await httpClient.PostAsync(
                $"{_options.ServerUrl}/realms/{_options.Realm}/protocol/openid-connect/token",
                new FormUrlEncodedContent(formData),
                ct);

        response.EnsureSuccessStatusCode();

        var token =
            await response.Content.ReadFromJsonAsync<
                AccessTokenResponse>(ct);

        return token!.Access_Token;
    }

    public async Task<string> CreateUserAsync(
    string token,
    string firstName,
    string lastName,
    string email,
    string password,
    CancellationToken ct)
    {
        var request = new CreateKeycloakUserRequest
        {
            Username = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Enabled = true,
            Credentials =
            [
                new Credential
            {
                Type = "password",
                Value = password,
                Temporary = false
            }
            ]
        };

        httpClient.DefaultRequestHeaders.Clear();

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response =
            await httpClient.PostAsJsonAsync(
                $"{_options.ServerUrl}/admin/realms/{_options.Realm}/users",
                request,
                ct);

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content.ReadAsStringAsync(ct);

            throw new Exception(
                $"Keycloak Error: {response.StatusCode} - {error}");
        }

        var location =
            response.Headers.Location?.ToString();

        return location?.Split('/').Last() ?? string.Empty;
    }

    public async Task AssignRoleAsync(
    string token,
    string userId,
    string roleName,
    CancellationToken ct)
    {
        httpClient.DefaultRequestHeaders.Clear();

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var role =
            await httpClient.GetFromJsonAsync<RoleRepresentation>(
                $"{_options.ServerUrl}/admin/realms/{_options.Realm}/roles/{roleName}",
                ct);

        if (role is null)
        {
            throw new Exception(
                $"Role '{roleName}' not found.");
        }

        var response =
            await httpClient.PostAsJsonAsync(
                $"{_options.ServerUrl}/admin/realms/{_options.Realm}/users/{userId}/role-mappings/realm",
                new[]
                {
                role
                },
                ct);

        response.EnsureSuccessStatusCode();
    }

    public async Task<string> GetUserRoleAsync(
    string token,
    string userId,
    CancellationToken ct)
    {
        httpClient.DefaultRequestHeaders.Clear();

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var roles =
            await httpClient.GetFromJsonAsync<
                List<RoleRepresentation>>(
                $"{_options.ServerUrl}/admin/realms/{_options.Realm}/users/{userId}/role-mappings/realm",
                ct);

        return roles?
            .FirstOrDefault(x =>
                !x.Name.StartsWith("default-roles"))
            ?.Name
            ?? string.Empty;

    }
    public async Task UpdateUserAsync(
    string token,
    string userId,
    string firstName,
    string lastName,
    string email,
    CancellationToken ct)
    {
        httpClient.DefaultRequestHeaders.Clear();

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var request = new
        {
            firstName,
            lastName,
            email
        };

        var response =
            await httpClient.PutAsJsonAsync(
                $"{_options.ServerUrl}/admin/realms/{_options.Realm}/users/{userId}",
                request,
                ct);

        // response.EnsureSuccessStatusCode();
        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content.ReadAsStringAsync(ct);

            throw new Exception(
                $"Keycloak Error: {response.StatusCode} - {error}");
        }
    }


    public async Task RemoveRoleAsync(
    string token,
    string userId,
    string roleName,
    CancellationToken ct)
    {
        httpClient.DefaultRequestHeaders.Clear();

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var role =
            await httpClient.GetFromJsonAsync<RoleRepresentation>(
                $"{_options.ServerUrl}/admin/realms/{_options.Realm}/roles/{roleName}",
                ct);

        if (role is null)
        {
            return;
        }

        var request =
            new HttpRequestMessage(
                HttpMethod.Delete,
                $"{_options.ServerUrl}/admin/realms/{_options.Realm}/users/{userId}/role-mappings/realm")
            {
                Content = JsonContent.Create(
                    new[] { role })
            };

        var response =
            await httpClient.SendAsync(
                request,
                ct);

        response.EnsureSuccessStatusCode();
    }


    public async Task UpdateUserRoleAsync(
    string token,
    string userId,
    string roleName,
    CancellationToken ct)
    {
        var currentRole =
            await GetUserRoleAsync(
                token,
                userId,
                ct);

        if (!string.IsNullOrWhiteSpace(currentRole))
        {
            await RemoveRoleAsync(
                token,
                userId,
                currentRole,
                ct);
        }

        await AssignRoleAsync(
            token,
            userId,
            roleName.ToLower(),
            ct);
    }

    public async Task DisableUserAsync(
    string token,
    string userId,
    CancellationToken ct)
    {
        httpClient.DefaultRequestHeaders.Clear();

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var request = new
        {
            enabled = false
        };

        var response =
            await httpClient.PutAsJsonAsync(
                $"{_options.ServerUrl}/admin/realms/{_options.Realm}/users/{userId}",
                request,
                ct);

        response.EnsureSuccessStatusCode();
    }

    public async Task<List<string>> GetRolesAsync(
    string token,
    CancellationToken ct)
    {
        httpClient.DefaultRequestHeaders.Clear();

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token);

        var roles =
            await httpClient.GetFromJsonAsync<
                List<RoleRepresentation>>(
                $"{_options.ServerUrl}/admin/realms/{_options.Realm}/roles",
                ct);

        return roles?
            .Select(x => x.Name)
            .Where(x =>
                !string.IsNullOrWhiteSpace(x) &&
                !x.StartsWith("default-roles"))
            .ToList()
            ?? [];
    }
}
