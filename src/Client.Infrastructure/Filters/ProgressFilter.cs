using System;

namespace EDO_FOMS.Client.Infrastructure.Filters;

public class ProgressFilter : BaseFilter
{
    public int? OrgId { get; set; } = null;
    public string UserId { get; set; } = null;

    //public bool StageDraft { get; set; } = false;
    //public bool StageInProgress { get; set; } = false;
    //public bool StageRejected { get; set; } = false;

    public bool TypeContract { get; set; } = false;
    public bool TypeAgreement { get; set; } = false;

    public string TextNumber { get; set; } = "";
    public string TextTitle { get; set; } = "";

    public DateTime? DateFrom { get; set; } = null;
    public DateTime? DateTo { get; set; } = null;

    public bool ChangedAttendee { get; set; } = false;
    public bool ChangedStage { get; set; } = false;
    public bool ChangedType { get; set; } = false;
    public bool ChangedText { get; set; } = false;
    public bool ChangedDate { get; set; } = false;
}
