using EDO_FOMS.Application.Specifications.Base;
using EDO_FOMS.Domain.Entities.Dir;

namespace EDO_FOMS.Application.Specifications.Dir;

public class DocTypesFilterSpecification : EdoFomsSpecification<DocumentType>
{
    public DocTypesFilterSpecification(string searchString, bool matchCase = false)
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            Criteria = _ => true;
        }
        else
        {
            var search = matchCase ? searchString : searchString.ToUpper();

            Criteria = p => matchCase
                ? p.Short.Contains(search) || p.Label.Contains(search) || p.Name.Contains(search)
                    || p.NameEn.Contains(search) || p.Description.Contains(search)
                : p.Short.ToUpper().Contains(search) || p.Label.ToUpper().Contains(search) || p.Name.ToUpper().Contains(search)
                    || p.NameEn.ToUpper().Contains(search) || p.Description.ToUpper().Contains(search);
        }
    }
}
