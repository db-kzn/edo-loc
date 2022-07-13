using EDO_FOMS.Infrastructure.Models.Identity;
using EDO_FOMS.Application.Specifications.Base;
using EDO_FOMS.Application.Requests.Admin;
using System;
using System.Linq.Expressions;
using System.Linq;

namespace EDO_FOMS.Infrastructure.Specifications
{
    public class UserFilterSpecification : EdoFomsSpecification<EdoFomsUser>
    {
        public UserFilterSpecification(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                Criteria = p => p.GivenName.Contains(searchString) || p.Surname.Contains(searchString)
                    || p.Email.Contains(searchString) || p.PhoneNumber.Contains(searchString)
                    || p.UserName.Contains(searchString) || p.Title.Contains(searchString);
            }
            else
            {
                Criteria = _ => true;
            }
        }

        public UserFilterSpecification(GetPagedUsersRequest request)
        {
            Criteria = p => !p.IsDeleted;

            if (!string.IsNullOrEmpty(request.SearchString))
            {
                var search = request.MatchCase ? request.SearchString : request.SearchString.ToUpper();

                Expression<Func<EdoFomsUser, bool>> c = p => request.MatchCase
                    ? p.GivenName.Contains(search) || p.Surname.Contains(search) || p.Title.Contains(search)
                        || p.Email.Contains(search) || p.PhoneNumber.Contains(search) || p.UserName.Contains(search)
                    : p.GivenName.ToUpper().Contains(search) || p.Surname.ToUpper().Contains(search) || p.Title.ToUpper().Contains(search)
                        || p.Email.ToUpper().Contains(search) || p.PhoneNumber.ToUpper().Contains(search) || p.UserName.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }
        }

        public UserFilterSpecification(SearchUsersRequest request)
        {
            Criteria = p => !p.IsDeleted;

            if (request.OrgTypes.Length > 0 && request.OrgTypes.Length < 3)
            {
                Expression<Func<EdoFomsUser, bool>> c = p => request.OrgTypes.Contains(p.OrgType);
                Criteria = Criteria.And(c);
            }

            if (request.UserBaseRoles.Length > 0 && request.UserBaseRoles.Length < 5)
            {
                Expression<Func<EdoFomsUser, bool>> c = p => request.UserBaseRoles.Contains(p.BaseRole);
                Criteria = Criteria.And(c);
            }

            if (request.EmailConfirmed != null)
            {
                Expression<Func<EdoFomsUser, bool>> c = p => request.EmailConfirmed == p.EmailConfirmed;
                Criteria = Criteria.And(c);
            }

            if (request.PhoneConfirmed != null)
            {
                Expression<Func<EdoFomsUser, bool>> c = p => request.PhoneConfirmed == p.PhoneNumberConfirmed;
                Criteria = Criteria.And(c);
            }

            if (request.UserIsActive != null)
            {
                Expression<Func<EdoFomsUser, bool>> c = p => request.UserIsActive == p.IsActive;
                Criteria = Criteria.And(c);
            }

            if (!string.IsNullOrEmpty(request.SearchString))
            {
                var search = request.MatchCase ? request.SearchString : request.SearchString.ToUpper();

                Expression<Func<EdoFomsUser, bool>> c = p => request.MatchCase
                    ? p.GivenName.Contains(search) || p.Surname.Contains(search) || p.Title.Contains(search)
                        || p.Email.Contains(search) || p.PhoneNumber.Contains(search) || p.UserName.Contains(search)
                    : p.GivenName.ToUpper().Contains(search) || p.Surname.ToUpper().Contains(search) || p.Title.ToUpper().Contains(search)
                        || p.Email.ToUpper().Contains(search) || p.PhoneNumber.ToUpper().Contains(search) || p.UserName.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (!string.IsNullOrEmpty(request.TextInnLe))
            {
                var search = request.MatchCase ? request.TextInnLe : request.TextInnLe.ToUpper();

                Expression<Func<EdoFomsUser, bool>> c = p => request.MatchCase
                   ? p.InnLe.Contains(search) : p.InnLe.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (!string.IsNullOrEmpty(request.TextSnils))
            {
                var search = request.MatchCase ? request.TextSnils : request.TextSnils.ToUpper();

                Expression<Func<EdoFomsUser, bool>> c = p => request.MatchCase
                   ? p.Snils.Contains(search) : p.Snils.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (!string.IsNullOrEmpty(request.TextTitle))
            {
                var search = request.MatchCase ? request.TextTitle : request.TextTitle.ToUpper();

                Expression<Func<EdoFomsUser, bool>> c = p => request.MatchCase
                   ? p.Title.Contains(search) : p.Title.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (!string.IsNullOrEmpty(request.TextSurname))
            {
                var search = request.MatchCase ? request.TextSurname : request.TextSurname.ToUpper();

                Expression<Func<EdoFomsUser, bool>> c = p => request.MatchCase
                   ? p.Surname.Contains(search) : p.Surname.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (!string.IsNullOrEmpty(request.TextGivenName))
            {
                var search = request.MatchCase ? request.TextGivenName : request.TextGivenName.ToUpper();

                Expression<Func<EdoFomsUser, bool>> c = p => request.MatchCase
                   ? p.GivenName.Contains(search) : p.GivenName.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (!string.IsNullOrEmpty(request.TextEmail))
            {
                var search = request.MatchCase ? request.TextEmail : request.TextEmail.ToUpper();

                Expression<Func<EdoFomsUser, bool>> c = p => request.MatchCase
                   ? p.Email.Contains(search) : p.Email.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (!string.IsNullOrEmpty(request.TextPhone))
            {
                var search = request.MatchCase ? request.TextPhone : request.TextPhone.ToUpper();

                Expression<Func<EdoFomsUser, bool>> c = p => request.MatchCase
                   ? p.PhoneNumber.Contains(search) : p.PhoneNumber.ToUpper().Contains(search);

                Criteria = Criteria.And(c);
            }

            if (request.CreateOnFrom != null)
            {
                Expression<Func<EdoFomsUser, bool>> c = p => p.CreatedOn >= request.CreateOnFrom;
                Criteria = Criteria.And(c);
            }

            if (request.CreateOnTo != null)
            {
                Expression<Func<EdoFomsUser, bool>> c = p => p.CreatedOn <= request.CreateOnTo;
                Criteria = Criteria.And(c);
            }
        }
    }
}