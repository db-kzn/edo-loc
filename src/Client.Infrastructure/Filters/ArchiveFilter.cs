using EDO_FOMS.Client.Infrastructure.Models.Dirs;
using System;
using System.Collections.Generic;

namespace EDO_FOMS.Client.Infrastructure.Filters;

public class ArchiveFilter : BaseFilter
{
    public int? OrgId { get; set; } = null;
    public string UserId { get; set; } = null;

    public bool ActionToRun { get; set; } = false;
    public bool ActionToSign { get; set; } = false;
    public bool ActionToApprove { get; set; } = false;
    public bool ActionToVerify { get; set; } = false;

    //public bool TypeContract { get; set; } = false;
    //public bool TypeAgreement { get; set; } = false;
    public List<FilterDocTypeModel> DocTypes { get; set; } = new();

    public string TextNumber { get; set; } = "";
    public string TextTitle { get; set; } = "";

    public DateTime? DateFrom { get; set; } = null;
    public DateTime? DateTo { get; set; } = null;

    public bool ChangedAttendee { get; set; } = false;
    public bool ChangedAction { get; set; } = false;
    public bool ChangedType { get; set; } = false;
    public bool ChangedText { get; set; } = false;
    public bool ChangedDate { get; set; } = false;
}
