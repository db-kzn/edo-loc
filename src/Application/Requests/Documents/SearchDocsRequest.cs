using EDO_FOMS.Domain.Enums;
using System;

namespace EDO_FOMS.Application.Requests.Documents;

public class SearchDocsRequest : PagedRequest
{
    public DocStages[] DocStages { get; set; }
    public int[] DocTypeIds { get; set; }

    public bool MatchCase { get; set; }
    public string SearchString { get; set; }

    public int? OrgId { get; set; }
    public string UserId { get; set; }
    public string TextNumber { get; set; } = "";
    public string TextTitle { get; set; } = "";

    public DateTime? DateFrom { get; set; } = null;
    public DateTime? DateTo { get; set; } = null;
    public DateTime? CreateOnFrom { get; set; } = null;
    public DateTime? CreateOnTo { get; set; } = null;
}
