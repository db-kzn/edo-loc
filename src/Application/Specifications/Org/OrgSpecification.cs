using EDO_FOMS.Application.Requests.Orgs;
using EDO_FOMS.Application.Specifications.Base;
using EDO_FOMS.Domain.Entities.Org;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EDO_FOMS.Application.Specifications.Org
{
    public class OrgSpecification : EdoFomsSpecification<Organization>
    {
        public OrgSpecification(string searchString, string userId)
        {
            Criteria = p => p.IsPublic || p.CreatedBy == userId;
            
            if (!string.IsNullOrEmpty(searchString))
            {
                var search = searchString.ToUpper();

                Criteria = p => p.Inn.ToUpper().Contains(search) || p.Ogrn.ToUpper().Contains(search)
                        || p.Name.ToUpper().Contains(search) || p.ShortName.ToUpper().Contains(search)
                        || p.Email.ToUpper().Contains(search) || p.Phone.ToUpper().Contains(search);
            }
        }

        public OrgSpecification(GetPagedOrgsRequest request)
        {
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                var search = request.MatchCase ? request.SearchString : request.SearchString.ToUpper();

                //p.Id.ToString().ToUpper().Contains(search) || 

                Criteria = p => request.MatchCase
                    ? p.Inn.Contains(search) || p.Ogrn.Contains(search)
                        || p.Name.Contains(search) || p.ShortName.Contains(search)
                        || p.Email.Contains(search) || p.Phone.Contains(search)
                    : p.Inn.ToUpper().Contains(search) || p.Ogrn.ToUpper().Contains(search)
                        || p.Name.ToUpper().Contains(search) || p.ShortName.ToUpper().Contains(search)
                        || p.Email.ToUpper().Contains(search) || p.Phone.ToUpper().Contains(search);
            }
            else
            {
                Criteria = _ => true;
            }
        }

        public OrgSpecification(SearchOrgsRequest request)
        {
            Criteria = _ => !(string.IsNullOrEmpty(request.TextOrgId) && string.IsNullOrEmpty(request.SearchString)
                && string.IsNullOrEmpty(request.TextInnLe) && string.IsNullOrEmpty(request.TextOgrn)
                && string.IsNullOrEmpty(request.TextName) && string.IsNullOrEmpty(request.TextShortName)
                && string.IsNullOrEmpty(request.TextEmail) && string.IsNullOrEmpty(request.TextPhone)
                && (request.OrgTypes.Length == 0 || request.OrgTypes.Length > 3)
                && (request.OrgStates.Length == 0 || request.OrgStates.Length > 4)
                && request.CreateOnFrom == null && request.CreateOnTo == null);

            if (!string.IsNullOrEmpty(request.SearchString))
            {
                var search = request.MatchCase ? request.SearchString : request.SearchString.ToUpper();

                Expression<Func<Organization, bool>> c = p => request.MatchCase
                    ? p.Id.ToString().Contains(search) || p.Inn.Contains(search) || p.Ogrn.Contains(search)
                        || p.Name.Contains(search) || p.ShortName.Contains(search)
                        || p.Email.Contains(search) || p.Phone.Contains(search)
                    : p.Id.ToString().ToUpper().Contains(search) || p.Inn.ToUpper().Contains(search) || p.Ogrn.ToUpper().Contains(search)
                        || p.Name.ToUpper().Contains(search) || p.ShortName.ToUpper().Contains(search)
                        || p.Email.ToUpper().Contains(search) || p.Phone.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (!string.IsNullOrEmpty(request.TextOrgId))
            {
                var search = request.MatchCase ? request.TextOrgId : request.TextOrgId.ToUpper();

                Expression<Func<Organization, bool>> c = p => request.MatchCase
                   ? p.Id.ToString().Contains(search) : p.Id.ToString().ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (!string.IsNullOrEmpty(request.TextInnLe))
            {
                var search = request.MatchCase ? request.TextInnLe : request.TextInnLe.ToUpper();

                Expression<Func<Organization, bool>> c = p => request.MatchCase
                   ? p.Inn.Contains(search) : p.Inn.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (!string.IsNullOrEmpty(request.TextOgrn))
            {
                var search = request.MatchCase ? request.TextOgrn : request.TextOgrn.ToUpper();

                Expression<Func<Organization, bool>> c = p => request.MatchCase
                   ? p.Ogrn.Contains(search) : p.Ogrn.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (!string.IsNullOrEmpty(request.TextName))
            {
                var search = request.MatchCase ? request.TextName : request.TextName.ToUpper();

                Expression<Func<Organization, bool>> c = p => request.MatchCase
                   ? p.Name.Contains(search) : p.Name.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (!string.IsNullOrEmpty(request.TextShortName))
            {
                var search = request.MatchCase ? request.TextShortName : request.TextShortName.ToUpper();

                Expression<Func<Organization, bool>> c = p => request.MatchCase
                   ? p.ShortName.Contains(search) : p.ShortName.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (!string.IsNullOrEmpty(request.TextEmail))
            {
                var search = request.MatchCase ? request.TextEmail : request.TextEmail.ToUpper();

                Expression<Func<Organization, bool>> c = p => request.MatchCase
                   ? p.Email.Contains(search) : p.Email.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (!string.IsNullOrEmpty(request.TextPhone))
            {
                var search = request.MatchCase ? request.TextPhone : request.TextPhone.ToUpper();

                Expression<Func<Organization, bool>> c = p => request.MatchCase
                   ? p.Phone.Contains(search) : p.Phone.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (request.OrgTypes.Length > 0 && request.OrgTypes.Length <= 3)
            {
                Expression<Func<Organization, bool>> c = p => request.OrgTypes.Contains(p.Type);
                Criteria = Criteria.And(c);
            }

            if (request.OrgStates.Length > 0 && request.OrgStates.Length <= 4)
            {
                Expression<Func<Organization, bool>> c = p => request.OrgStates.Contains(p.State);
                Criteria = Criteria.And(c);
            }

            if (request.CreateOnFrom != null)
            {
                Expression<Func<Organization, bool>> c = p => p.CreatedOn >= request.CreateOnFrom;
                Criteria = Criteria.And(c);
            }

            if (request.CreateOnTo != null)
            {
                Expression<Func<Organization, bool>> c = p => p.CreatedOn <= request.CreateOnTo;
                Criteria = Criteria.And(c);
            }
        }
    }
}
