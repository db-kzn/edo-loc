using EDO_FOMS.Domain.Enums;
using System;

namespace EDO_FOMS.Application.Requests.Orgs;

public class SearchOrgsRequest : PagedRequest
{
    public string SearchString { get; set; }
    public bool MatchCase { get; set; }

    public string TextOrgId { get; set; }
    public string TextInnLe { get; set; }
    public string TextOgrn { get; set; }

    public string TextName { get; set; }
    public string TextShortName { get; set; }

    public string TextEmail { get; set; }
    public string TextPhone { get; set; }

    public OrgTypes[] OrgTypes { get; set; }
    public OrgStates[] OrgStates { get; set; }

    public DateTime? CreateOnFrom { get; set; }
    public DateTime? CreateOnTo { get; set; }
}
