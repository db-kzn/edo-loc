namespace EDO_FOMS.Client.Infrastructure.Filters;

public class OrgFilter : BaseFilter
{
    public string TextOrgId { get; set; } = "";
    public string TextInnLe { get; set; } = "";
    public string TextOgrn { get; set; } = "";

    public string TextName { get; set; } = "";
    public string TextShortName { get; set; } = "";
    public string TextEmail { get; set; } = "";
    public string TextPhone { get; set; } = "";

    public bool TypeMO { get; set; } = false;
    public bool TypeSMO { get; set; } = false;
    public bool TypeFund { get; set; } = false;

    public bool StateActive { get; set; } = false;
    public bool StateInactive { get; set; } = false;
    public bool StateOnSubmit { get; set; } = false;
    public bool StateBlocked { get; set; } = false;

    public bool ChangedText { get; set; } = false;
    public bool ChangedTypes { get; set; } = false;
    public bool ChangedStates { get; set; } = false;
}
