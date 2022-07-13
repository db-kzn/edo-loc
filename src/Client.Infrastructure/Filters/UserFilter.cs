namespace EDO_FOMS.Client.Infrastructure.Filters;

public class UserFilter : BaseFilter
{
    public string TextInnLe { get; set; } = "";
    public string TextSnils { get; set; } = "";

    public string TextTitle { get; set; } = "";
    public string TextSurname { get; set; } = "";
    public string TextGivenName { get; set; } = "";

    public string TextEmail { get; set; } = "";
    public string TextPhone { get; set; } = "";

    public bool? EmailConfirmed { get; set; } = null;
    public bool? PhoneConfirmed { get; set; } = null;
    public bool? UserIsActive { get; set; } = null;

    public bool TypeMO { get; set; } = false;
    public bool TypeSMO { get; set; } = false;
    public bool TypeFund { get; set; } = false;

    public bool RoleUser { get; set; } = false;
    public bool RoleEmployee { get; set; } = false;
    public bool RoleManager { get; set; } = false;
    public bool RoleChief { get; set; } = false;
    public bool RoleAdmin { get; set; } = false;

    public bool ChangedText { get; set; } = false;
    public bool ChangedStates { get; set; } = false;
    public bool ChangedTypes { get; set; } = false;
    public bool ChangedRoles { get; set; } = false;
}
