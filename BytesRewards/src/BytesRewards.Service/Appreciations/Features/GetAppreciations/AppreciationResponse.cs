namespace BytesRewards.Service.Appreciations.Features.GetAppreciations;

public sealed class AppreciationResponse
{
    public Guid Id { get; set; }

    public string FromUserName { get; set; } = string.Empty;

    public string ToUserName { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public int LikesCount { get; set; }

    public bool IsLiked { get; set; }
}