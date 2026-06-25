using AppWeaver.Mediator.Interfaces;
using AppWeaver.Results;
using BytesRewards.Service.Designations.Domain;
using BytesRewards.Service.Infrastructure;

namespace BytesRewards.Service.Designations.Features.CreateDesignation;

public sealed class CreateDesignationCommandHandler(ApplicationDbContext context)
    : ICommandHandler<CreateDesignationCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(
        CreateDesignationCommand request, CancellationToken ct)
    {
        var designation = new Designation
        {
            Id          = Guid.NewGuid(),
            Name        = request.Name,
            Description = request.Description,
            IsActive    = true,
            CreatedAt   = DateTime.UtcNow
        };

        context.Designations.Add(designation);
        await context.SaveChangesAsync(ct);

        return Result<Guid>.Ok(designation.Id);
    }
}
