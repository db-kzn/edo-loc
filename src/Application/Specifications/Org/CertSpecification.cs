using EDO_FOMS.Application.Requests.Admin;
using EDO_FOMS.Application.Specifications.Base;
using EDO_FOMS.Domain.Entities.Org;
using System;
using System.Linq.Expressions;

namespace EDO_FOMS.Application.Specifications.Org;

public class CertSpecification : EdoFomsSpecification<Certificate>
{
    public CertSpecification(string searchString, string userId)
    {
        Criteria = p => p.CreatedBy == userId;

        if (!string.IsNullOrEmpty(searchString))
        {
            var search = searchString.ToUpper();
            Criteria = p => p.Thumbprint.ToUpper().Contains(search) || p.Snils.ToUpper().Contains(search);
        }
    }

    public CertSpecification(GetPagedCertsRequest request)
    {
        if (!string.IsNullOrEmpty(request.SearchString))
        {
            var search = request.MatchCase ? request.SearchString : request.SearchString.ToUpper();

            Criteria = p => request.MatchCase
                ? p.Thumbprint.Contains(search) || p.Snils.Contains(search)
                : p.Thumbprint.ToUpper().Contains(search) || p.Snils.ToUpper().Contains(search);
        }
        else
        {
            Criteria = _ => true;
        }
    }

    public CertSpecification(SearchCertsRequest request)
    {
        Criteria = _ => !(string.IsNullOrEmpty(request.TextCertId)
            && string.IsNullOrEmpty(request.TextThumbPrint)
            && string.IsNullOrEmpty(request.TextSnils)
            && request.CertIsActive == null && request.SignAllowed == null
            && request.FromDateFrom == null && request.FromDateTo == null
            && request.TillDateFrom == null && request.TillDateTo == null
            && request.CreateOnFrom == null && request.CreateOnTo == null);

        if (!string.IsNullOrEmpty(request.SearchString))
        {
            var search = request.MatchCase ? request.SearchString : request.SearchString.ToUpper();

            Expression<Func<Certificate, bool>> c = p => request.MatchCase
                ? p.Id.ToString().Contains(search) || p.Thumbprint.Contains(search) || p.Snils.Contains(search)
                : p.Id.ToString().ToUpper().Contains(search) || p.Thumbprint.ToUpper().Contains(search) || p.Snils.ToUpper().Contains(search);
        
            Criteria = Criteria.And(c);
        }

        if (!string.IsNullOrEmpty(request.TextCertId))
        {
            var search = request.MatchCase ? request.TextCertId : request.TextCertId.ToUpper();

            Expression<Func<Certificate, bool>> c = p => request.MatchCase
               ? p.Id.ToString().Contains(search) : p.Id.ToString().ToUpper().Contains(search);

            Criteria = Criteria.And(c);
        }

        if (!string.IsNullOrEmpty(request.TextThumbPrint))
        {
            var search = request.MatchCase ? request.TextThumbPrint : request.TextThumbPrint.ToUpper();

            Expression<Func<Certificate, bool>> c = p => request.MatchCase
               ? p.Thumbprint.Contains(search) : p.Thumbprint.ToUpper().Contains(search);

            Criteria = Criteria.And(c);
        }

        if (!string.IsNullOrEmpty(request.TextSnils))
        {
            var search = request.MatchCase ? request.TextSnils : request.TextSnils.ToUpper();

            Expression<Func<Certificate, bool>> c = p => request.MatchCase
               ? p.Snils.Contains(search) : p.Snils.ToUpper().Contains(search);

            Criteria = Criteria.And(c);
        }

        if (request.CertIsActive != null)
        {
            Expression<Func<Certificate, bool>> c = p => request.CertIsActive == p.IsActive;
            Criteria = Criteria.And(c);
        }

        if (request.SignAllowed != null)
        {
            Expression<Func<Certificate, bool>> c = p => request.SignAllowed == p.SignAllowed;
            Criteria = Criteria.And(c);
        }

        if (request.FromDateFrom != null)
        {
            Expression<Func<Certificate, bool>> c = p => p.FromDate >= request.FromDateFrom;
            Criteria = Criteria.And(c);
        }

        if (request.FromDateTo != null)
        {
            Expression<Func<Certificate, bool>> c = p => p.FromDate <= request.FromDateTo;
            Criteria = Criteria.And(c);
        }

        if (request.TillDateFrom != null)
        {
            Expression<Func<Certificate, bool>> c = p => p.TillDate >= request.TillDateFrom;
            Criteria = Criteria.And(c);
        }

        if (request.TillDateTo != null)
        {
            Expression<Func<Certificate, bool>> c = p => p.TillDate <= request.TillDateTo;
            Criteria = Criteria.And(c);
        }

        if (request.CreateOnFrom != null)
        {
            Expression<Func<Certificate, bool>> c = p => p.CreatedOn >= request.CreateOnFrom;
            Criteria = Criteria.And(c);
        }

        if (request.CreateOnTo != null)
        {
            Expression<Func<Certificate, bool>> c = p => p.CreatedOn <= request.CreateOnTo;
            Criteria = Criteria.And(c);
        }
    }
}
