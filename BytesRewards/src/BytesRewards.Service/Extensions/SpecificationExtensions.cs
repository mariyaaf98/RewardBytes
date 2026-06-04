using AppWeaver.DomainAbstraction.Entities;
using Ardalis.Specification;

namespace BytesRewards.Service.Extensions;

public static class SpecificationExtensions
{
    // Generic order by Id ascending for any Specification<TEntity>
    public static TSpec OrderByIdAsc<TSpec, TEntity, TId>(this TSpec spec)
        where TSpec : Specification<TEntity>
        where TEntity : class, AppWeaver.DomainAbstraction.Entities.IEntity<TId>
        where TId : struct, IEquatable<TId>
    {
        spec.Query.OrderBy(e => e.Id);
        return spec;
    }

    // Generic order by Id descending for any Specification<TEntity>
    public static TSpec OrderByIdDesc<TSpec, TEntity, TId>(this TSpec spec)
        where TSpec : Specification<TEntity>
        where TEntity : class, AppWeaver.DomainAbstraction.Entities.IEntity<TId>
        where TId : struct, IEquatable<TId>
    {
        spec.Query.OrderByDescending(e => e.Id);
        return spec;
    }

    // Order by Name ascending for any named entity
    public static TSpec OrderByNameAsc<TSpec, TEntity>(this TSpec spec)
        where TSpec : Specification<TEntity>
        where TEntity : class, INamedEntity
    {
        spec.Query.OrderBy(e => e.Name);
        return spec;
    }
}

