using Microsoft.EntityFrameworkCore;
using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Designations.Features.DeleteDesignation;

public sealed class DeleteDesignationCommandHandler(ApplicationDbContext context)
    : ICommandHandler<DeleteDesignationCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        DeleteDesignationCommand request, CancellationToken ct)
    {
        var designation = await context.Designations
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (designation is null)
            return Result<bool>.Ok(false);

        designation.IsActive  = false;
        designation.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
