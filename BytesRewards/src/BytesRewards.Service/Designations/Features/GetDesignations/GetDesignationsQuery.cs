using AppWeaver.Mediator.Interfaces;

namespace BytesRewards.Service.Designations.Features.GetDesignations;

public sealed record GetDesignationsQuery : IQuery<List<DesignationResponse>>;
