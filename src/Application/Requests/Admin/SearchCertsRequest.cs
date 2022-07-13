using System;

namespace EDO_FOMS.Application.Requests.Admin;

public class SearchCertsRequest : PagedRequest
{
    public string SearchString { get; set; }
    public bool MatchCase { get; set; }

    public string TextCertId { get; set; }
    public string TextThumbPrint { get; set; }
    public string TextSnils { get; set; }

    public bool? CertIsActive { get; set; }
    public bool? SignAllowed { get; set; }

    public DateTime? FromDateFrom { get; set; }
    public DateTime? FromDateTo { get; set; }

    public DateTime? TillDateFrom { get; set; }
    public DateTime? TillDateTo { get; set; }

    public DateTime? CreateOnFrom { get; set; }
    public DateTime? CreateOnTo { get; set; }
}
