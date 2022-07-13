using EDO_FOMS.Application.Specifications.Base;
using EDO_FOMS.Domain.Entities.Dir;

namespace EDO_FOMS.Application.Specifications.Doc
{
    public class DocumentTypeFilterSpecification : EdoFomsSpecification<DocumentType>
    {
        public DocumentTypeFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.Name.Contains(searchString) || p.Description.Contains(searchString);
            }
            else
            {
                Criteria = p => true;
            }
        }
    }
}