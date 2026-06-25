namespace BytesRewards.Service.Designations.Features.CreateDesignation;

public sealed class CreateDesignationRequest
{
    public string Name        { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
