namespace BytesRewards.Service.Infrastructure.Security.Keycloak.Models;

public class CreateKeycloakUserRequest
{
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public bool Enabled { get; set; }

    public List<Credential> Credentials { get; set; } = [];
}

public class Credential
{
    public string Type { get; set; } = "password";

    public string Value { get; set; } = string.Empty;

    public bool Temporary { get; set; } = true;
}