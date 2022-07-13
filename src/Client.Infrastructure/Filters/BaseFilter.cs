using System;

namespace EDO_FOMS.Client.Infrastructure.Filters;

public class BaseFilter
{
    public bool IsActive { get; set; } = false;
    public bool IsEmpty { get; set; } = false;

    public DateTime? CreateOnFrom { get; set; } = null;
    public DateTime? CreateOnTo { get; set; } = null;
    public bool ChangedCreateOn { get; set; } = false;
}
