using System.Collections.Generic;

namespace EDO_FOMS.Domain.Contracts
{
    public abstract class AuditableEntityWithExtendedAttributes<TId, TEntityId, TEntity, TExtendedAttribute> 
        : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>
            where TEntity : IEntity<TEntityId>
    {
        public virtual ICollection<TExtendedAttribute> ExtendedAttributes { get; set; }

        public AuditableEntityWithExtendedAttributes()
        {
            ExtendedAttributes = new HashSet<TExtendedAttribute>();
        }
    }
}