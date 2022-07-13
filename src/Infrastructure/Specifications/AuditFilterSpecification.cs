using EDO_FOMS.Infrastructure.Models.Audit;
using EDO_FOMS.Application.Specifications.Base;

namespace EDO_FOMS.Infrastructure.Specifications
{
    public class AuditFilterSpecification : EdoFomsSpecification<Audit>
    {
        public AuditFilterSpecification(string userId, string searchString, bool searchInOldValues, bool searchInNewValues)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => (p.TableName.Contains(searchString) || searchInOldValues && p.OldValues.Contains(searchString) || searchInNewValues && p.NewValues.Contains(searchString)) && p.UserId == userId;
            }
            else
            {
                Criteria = p => p.UserId == userId;
            }
        }
    }
}