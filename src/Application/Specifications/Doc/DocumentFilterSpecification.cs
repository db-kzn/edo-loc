using EDO_FOMS.Application.Requests.Documents;
using EDO_FOMS.Application.Specifications.Base;
using EDO_FOMS.Domain.Entities.Doc;
using EDO_FOMS.Domain.Enums;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EDO_FOMS.Application.Specifications.Doc;

public class DocumentFilterSpecification : EdoFomsSpecification<Document>
{
    public DocumentFilterSpecification(string searchString, string userId, DocStages docStage = DocStages.Undefined, bool matchCase = false)
    {
        Criteria = p => p.IsPublic || p.EmplId == userId;

        if (docStage == DocStages.AllActive)
        {
            DocStages[] stages = {DocStages.Draft, DocStages.InProgress, DocStages.Rejected, DocStages.Agreed};

            Expression<Func<Document, bool>> c = p => stages.Contains(p.Stage);

            Criteria = Criteria.And(c);
        }
        else if (docStage != DocStages.Undefined)
        {
            Expression<Func<Document, bool>> c = p => p.Stage == docStage;
            Criteria = Criteria.And(c);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            var search = matchCase ? searchString : searchString.ToUpper();

            Expression <Func<Document, bool>> c = p => matchCase
                ? p.Title.Contains(search) || p.Number.Contains(search)
                : p.Title.ToUpper().Contains(search) || p.Number.ToUpper().Contains(search);

            Criteria = Criteria.And(c);
        }
    }

    public DocumentFilterSpecification(SearchDocsRequest request, string userId)
    {
        Criteria = p => p.IsPublic || p.EmplId == userId;

        if (request.DocStages.Length > 0)
        {
            Expression<Func<Document, bool>> c = p => request.DocStages.Contains(p.Stage);
            Criteria = Criteria.And(c);
        }
        else
        {
            Expression<Func<Document, bool>> c = p => p.Stage == DocStages.Undefined;
            Criteria = Criteria.And(c);
        }

        if (request.DocTypeIds.Length == 1)
        {
            var typeId = request.DocTypeIds[0];
            Expression<Func<Document, bool>> c = p => p.TypeId == typeId;
            Criteria = Criteria.And(c);
        }

        if (!string.IsNullOrEmpty(request.SearchString))
        {
            var search = request.MatchCase ? request.SearchString : request.SearchString.ToUpper();

            Expression<Func<Document, bool>> c = p => request.MatchCase
               ? p.Title.Contains(search) || p.Number.Contains(search)
               : p.Title.ToUpper().Contains(search) || p.Number.ToUpper().Contains(search);

            Criteria = Criteria.And(c);
        }

        if (!string.IsNullOrEmpty(request.TextNumber))
        {
            var search = request.MatchCase ? request.TextNumber : request.TextNumber.ToUpper();

            Expression<Func<Document, bool>> c = p => request.MatchCase
               ? p.Number.Contains(search) : p.Number.ToUpper().Contains(search);

            Criteria = Criteria.And(c);
        }

        if (!string.IsNullOrEmpty(request.TextTitle))
        {
            var search = request.MatchCase ? request.TextTitle : request.TextTitle.ToUpper();

            Expression<Func<Document, bool>> c = p => request.MatchCase
               ? p.Title.Contains(search) : p.Title.ToUpper().Contains(search);

            Criteria = Criteria.And(c);
        }

        if (request.DateFrom != null)
        {
            Expression<Func<Document, bool>> c = p => p.Date >= request.DateFrom;
            Criteria = Criteria.And(c);
        }

        if (request.DateTo != null)
        {
            Expression<Func<Document, bool>> c = p => p.Date <= request.DateTo;
            Criteria = Criteria.And(c);
        }

        if (request.CreateOnFrom != null)
        {
            Expression<Func<Document, bool>> c = p => p.CreatedOn >= request.CreateOnFrom;
            Criteria = Criteria.And(c);
        }

        if (request.CreateOnTo != null)
        {
            Expression<Func<Document, bool>> c = p => p.CreatedOn <= request.CreateOnTo;
            Criteria = Criteria.And(c);
        }
    }
}
