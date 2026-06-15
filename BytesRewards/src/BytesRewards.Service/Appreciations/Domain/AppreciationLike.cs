public class AppreciationLike
{
    public Guid Id { get; set; }

    public Guid AppreciationId { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; }
}