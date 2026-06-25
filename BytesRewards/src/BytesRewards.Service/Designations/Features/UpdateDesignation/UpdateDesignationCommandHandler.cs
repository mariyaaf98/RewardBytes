using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BytesRewards.Service.Designations.Features.UpdateDesignation;

public sealed class UpdateDesignationCommandHandler(ApplicationDbContext context)
    : ICommandHandler<UpdateDesignationCommand, Result<bool>>
{
    public async ValueTask<Result<bool>> Handle(
        UpdateDesignationCommand request, CancellationToken ct)
    {
        var designation = await context.Designations
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (designation is null)
            return Result<bool>.Ok(false);

        designation.Name        = request.Name;
        designation.Description = request.Description;
        designation.UpdatedAt   = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return Result<bool>.Ok(true);
    }
}
