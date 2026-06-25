namespace BytesRewards.Service.Users.Features.GetUserLookup;

public sealed class UserLookupResponse
{
    public Guid   Id              { get; set; }
    public string FullName        { get; set; } = string.Empty;
    public string DesignationName { get; set; } = string.Empty;
    public string DepartmentName  { get; set; } = string.Empty;
}