using EDO_FOMS.Domain.Enums;
using System;

namespace EDO_FOMS.Application.Requests.Admin;

public class SearchUsersRequest : PagedRequest
{
    public string SearchString { get; set; }
    public bool MatchCase { get; set; }

    public string TextInnLe { get; set; }
    public string TextSnils { get; set; }

    public string TextTitle { get; set; }
    public string TextSurname { get; set; }
    public string TextGivenName { get; set; }

    public string TextEmail { get; set; }
    public string TextPhone { get; set; }

    public bool? EmailConfirmed { get; set; }
    public bool? PhoneConfirmed { get; set; }
    public bool? UserIsActive { get; set; }

    public OrgTypes[] OrgTypes { get; set; }
    public UserBaseRoles[] UserBaseRoles { get; set; }


    public DateTime? CreateOnFrom { get; set; }
    public DateTime? CreateOnTo { get; set; }
}
