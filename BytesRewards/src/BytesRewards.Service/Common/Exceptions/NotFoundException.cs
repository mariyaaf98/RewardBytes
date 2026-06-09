namespace BytesRewards.Service.Common.Exceptions;

public sealed class NotFoundException
    : Exception
{
    public NotFoundException(
        string message)
        : base(message)
    {
    }
}