using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Domain.Entities.Dir;

[Index("Code"), Index("Inn"), Index("TfOkato")]
public class Company : AuditableEntity<int>
{
    public OrgTypes Type { get; set; } = OrgTypes.Undefined;
    public OrgStates State { get; set; } = OrgStates.Undefined;
    //public int? Region { get; set; } = null;
    [MaxLength(5)]
    public string TfOkato { get; set; } = "";

    [MaxLength(6)]
    public string Code { get; set; } = "";
    [MaxLength(12)]
    public string Inn { get; set; } = "";
    [MaxLength(9)]
    public string Kpp { get; set; } = "";
    [MaxLength(15)]
    public string Ogrn { get; set; } = "";

    [MaxLength(255)]
    public string Name { get; set; } = "";
    [MaxLength(255)]
    public string ShortName { get; set; } = "";
    [MaxLength(255)]
    public string Address { get; set; } = "";
    public Guid? AO { get; set; } = null;

    [MaxLength(50)]
    public string Phone { get; set; } = "";
    [MaxLength(50)]
    public string Fax { get; set; } = "";
    [MaxLength(13)]
    public string HotLine { get; set; } = "";
    [MaxLength(60)]
    public string Email { get; set; } = "";
    [MaxLength(100)]
    public string SiteUrl { get; set; } = "";

    [MaxLength(50)]
    public string HeadName { get; set; } = "";
    [MaxLength(50)]
    public string HeadLastName { get; set; } = "";
    [MaxLength(50)]
    public string HeadMidName { get; set; } = "";
    public DateTime? Changed { get; set; } = null;
}
