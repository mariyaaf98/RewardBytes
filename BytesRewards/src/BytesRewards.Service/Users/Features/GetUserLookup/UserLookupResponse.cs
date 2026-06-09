namespace BytesRewards.Service.Users.Features.GetUserLookup;

public sealed class UserLookupResponse
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;
}