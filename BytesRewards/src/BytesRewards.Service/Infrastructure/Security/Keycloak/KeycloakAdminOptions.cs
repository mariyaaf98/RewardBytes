namespace BytesRewards.Service.Infrastructure.Security.Keycloak;

public sealed class KeycloakAdminOptions
{
    public string ServerUrl { get; set; } = string.Empty;

    public string Realm { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;
}