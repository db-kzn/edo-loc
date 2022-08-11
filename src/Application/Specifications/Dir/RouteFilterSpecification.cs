using EDO_FOMS.Application.Specifications.Base;
using EDO_FOMS.Domain.Entities.Dir;

namespace EDO_FOMS.Application.Specifications.Dir;

public class RouteFilterSpecification : EdoFomsSpecification<Route>
{
    public RouteFilterSpecification(string searchString, bool matchCase = false)
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            Criteria = _ => true;
        }
        else
        {
            var search = matchCase ? searchString : searchString.ToUpper();
            Criteria = p => matchCase ? p.Name.Contains(search) : p.Name.ToUpper().Contains(search);
        }
    }
}
