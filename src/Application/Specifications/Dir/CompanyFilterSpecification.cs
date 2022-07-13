using EDO_FOMS.Application.Specifications.Base;
using EDO_FOMS.Domain.Entities.Dir;

namespace EDO_FOMS.Application.Specifications.Dir;

public class CompanyFilterSpecification : EdoFomsSpecification<Company>
{
    public CompanyFilterSpecification(string searchString, bool matchCase = false)
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            Criteria = _ => true;
        }
        else
        {
            var search = matchCase ? searchString : searchString.ToUpper();

            Criteria = p => matchCase
                ? p.Code.Contains(search) || p.Inn.Contains(search)
                    || p.Name.Contains(search) || p.ShortName.Contains(search) || p.Address.Contains(search)
                    || p.HeadLastName.Contains(search) || p.HeadName.Contains(search) || p.HeadMidName.Contains(search)
                    || p.Phone.Contains(search) || p.Fax.Contains(search) || p.HotLine.Contains(search)
                    || p.Email.Contains(search) || p.SiteUrl.Contains(search)
                : p.Code.Contains(search) || p.Inn.Contains(search)
                    || p.Name.ToUpper().Contains(search) || p.ShortName.ToUpper().Contains(search) || p.Address.ToUpper().Contains(search)
                    || p.HeadLastName.ToUpper().Contains(search) || p.HeadName.ToUpper().Contains(search) || p.HeadMidName.ToUpper().Contains(search)
                    || p.Phone.Contains(search) || p.Fax.Contains(search) || p.HotLine.Contains(search)
                    || p.Email.ToUpper().Contains(search) || p.SiteUrl.ToUpper().Contains(search);
        }
    }
}
