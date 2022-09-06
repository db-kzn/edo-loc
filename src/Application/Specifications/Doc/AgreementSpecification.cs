using EDO_FOMS.Application.Requests.Agreements;
using EDO_FOMS.Application.Specifications.Base;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Enums;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EDO_FOMS.Application.Specifications.Doc;

public class AgreementSpecification : EdoFomsSpecification<Agreement>
{
    public AgreementSpecification(string searchString, string userId, AgreementStates agrState = AgreementStates.Undefined, bool matchCase = false)
    {
        Criteria = p => p.EmplId == userId;

        if (agrState == AgreementStates.AllActive)
        {
            Expression<Func<Agreement, bool>> c = p => p.Document.Stage == DocStages.InProgress
                && p.State != AgreementStates.Control && p.State != AgreementStates.Expecting;

            Criteria = Criteria.And(c);
        }
        else if (agrState == AgreementStates.DocInArchive)
        {
            Expression<Func<Agreement, bool>> c = p => p.Document.Stage == DocStages.Archive;
            Criteria = Criteria.And(c);
        }

        // AgreementStates.DocInArchive

        if (!string.IsNullOrEmpty(searchString))
        {
            var search = matchCase ? searchString : searchString.ToUpper();

            Expression<Func<Agreement, bool>> c = p => matchCase
               ? p.Document.Title.Contains(search) || p.Document.Number.Contains(search)
               : p.Document.Title.ToUpper().Contains(search) || p.Document.Number.ToUpper().Contains(search);

            Criteria = Criteria.And(c);
        }
    }

    public AgreementSpecification(SearchAgrsRequest request, string userId)
    {
        Criteria = p => p.EmplId == userId && p.Document.Stage == request.DocStage;

        if (request.DocStage != DocStages.Archive)
        {
            Expression<Func<Agreement, bool>> c = p =>
                p.State != AgreementStates.Control && p.State != AgreementStates.Expecting;

            Criteria = Criteria.And(c);
        }

        //if (request.AgrActions?.Length > 0 && request.AgrActions?.Length < 4)
        //{
        //    Expression<Func<Agreement, bool>> c = p => request.AgrActions.Contains(p.Action);
        //    Criteria = Criteria.And(c);
        //}

        if (request.DocTypeIds.Length == 1)
        {
            var typeId = request.DocTypeIds[0];
            Expression<Func<Agreement, bool>> c = p => p.Document.TypeId == typeId;
            Criteria = Criteria.And(c);
        }

        if (!string.IsNullOrEmpty(request.SearchString))
        {
            var search = request.MatchCase ? request.SearchString : request.SearchString.ToUpper();

            Expression<Func<Agreement, bool>> c = p => request.MatchCase
               ? p.Document.Title.Contains(search) || p.Document.Number.Contains(search)
               : p.Document.Title.ToUpper().Contains(search) || p.Document.Number.ToUpper().Contains(search);

            Criteria = Criteria.And(c);
        }

        if (!string.IsNullOrEmpty(request.TextNumber))
        {
            var search = request.MatchCase ? request.TextNumber : request.TextNumber.ToUpper();

            Expression<Func<Agreement, bool>> c = p => request.MatchCase
               ? p.Document.Number.Contains(search) : p.Document.Number.ToUpper().Contains(search);

            Criteria = Criteria.And(c);
        }

        if (!string.IsNullOrEmpty(request.TextTitle))
        {
            var search = request.MatchCase ? request.TextTitle : request.TextTitle.ToUpper();

            Expression<Func<Agreement, bool>> c = p => request.MatchCase
               ? p.Document.Title.Contains(search) : p.Document.Title.ToUpper().Contains(search);

            Criteria = Criteria.And(c);
        }

        if (request.DateFrom != null)
        {
            Expression<Func<Agreement, bool>> c = p => p.Document.Date >= request.DateFrom;
            Criteria = Criteria.And(c);
        }

        if (request.DateTo != null)
        {
            Expression<Func<Agreement, bool>> c = p => p.Document.Date <= request.DateTo;
            Criteria = Criteria.And(c);
        }

        if (request.CreateOnFrom != null)
        {
            Expression<Func<Agreement, bool>> c = p => p.Document.CreatedOn >= request.CreateOnFrom;
            Criteria = Criteria.And(c);
        }

        if (request.CreateOnTo != null)
        {
            Expression<Func<Agreement, bool>> c = p => p.Document.CreatedOn <= request.CreateOnTo;
            Criteria = Criteria.And(c);
        }
    }
}
