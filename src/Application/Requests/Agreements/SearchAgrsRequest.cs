using EDO_FOMS.Domain.Enums;
using System;

namespace EDO_FOMS.Application.Requests.Agreements;

public class SearchAgrsRequest : PagedRequest
{
    public DocStages DocStage { get; set; }
    public AgreementActions[] AgrActions { get; set; }
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
