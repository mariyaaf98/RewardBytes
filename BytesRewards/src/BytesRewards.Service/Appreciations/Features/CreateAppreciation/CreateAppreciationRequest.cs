namespace BytesRewards.Service.Appreciations.Features.CreateAppreciation;

public sealed class CreateAppreciationRequest
{
    public Guid FromUserId { get; set; }

    public Guid ToUserId { get; set; }

    public string Message { get; set; } = string.Empty;
}