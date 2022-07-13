using System;

namespace EDO_FOMS.Client.Infrastructure.Filters;

public class CertFilter : BaseFilter
{
    public string TextCertId { get; set; } = "";
    public string TextThumbPrint { get; set; } = "";
    public string TextSnils { get; set; } = "";

    public bool? CertIsActive { get; set; } = null;
    public bool? SignAllowed { get; set; } = null;

    public DateTime? FromDateFrom { get; set; } = null;
    public DateTime? FromDateTo { get; set; } = null;

    public DateTime? TillDateFrom { get; set; } = null;
    public DateTime? TillDateTo { get; set; } = null;

    public bool ChangedText { get; set; } = false;
    public bool ChangedStates { get; set; } = false;
    public bool ChangedFromDate { get; set; } = false;
    public bool ChangedTillDate { get; set; } = false;
}
