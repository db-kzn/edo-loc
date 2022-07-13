using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EDO_FOMS.Application.Features.ExtendedAttributes.Commands.AddEdit;
using EDO_FOMS.Application.Features.ExtendedAttributes.Queries.Export;
using EDO_FOMS.Application.Features.ExtendedAttributes.Queries.GetAll;
using EDO_FOMS.Application.Features.ExtendedAttributes.Queries.GetAllByEntityId;
using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Shared.Wrapper;

namespace EDO_FOMS.Client.Infrastructure.Managers.ExtendedAttribute
{
    public interface IExtendedAttributeManager<TId, TEntityId, TEntity, TExtendedAttribute>
        where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
        where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
        where TId : IEquatable<TId>
    {
        Task<IResult<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>> GetAllAsync();

        Task<IResult<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>> GetAllByEntityIdAsync(TEntityId entityId);

        Task<IResult<TId>> SaveAsync(AddEditExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute> request);

        Task<IResult<TId>> DeleteAsync(TId id);

        Task<IResult<string>> ExportToExcelAsync(ExportExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute> request);
    }
}