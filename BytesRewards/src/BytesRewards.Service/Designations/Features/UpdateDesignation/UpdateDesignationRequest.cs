namespace BytesRewards.Service.Designations.Features.UpdateDesignation;

public sealed class UpdateDesignationRequest
{
    public Guid   Id          { get; set; }
    public string Name        { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
