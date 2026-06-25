namespace BytesRewards.Service.Designations.Features.GetDesignations;

public sealed class DesignationResponse
{
    public Guid   Id          { get; set; }
    public string Name        { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool   IsActive    { get; set; }
}
