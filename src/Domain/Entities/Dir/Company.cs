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
    public int? Region { get; set; } = null;
    [MaxLength(5)]
    public string TfOkato { get; set; } = string.Empty;

    [MaxLength(6)]
    public string Code { get; set; } = string.Empty;
    [MaxLength(12)]
    public string Inn { get; set; } = string.Empty;
    [MaxLength(9)]
    public string Kpp { get; set; } = string.Empty;
    [MaxLength(15)]
    public string Ogrn { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(255)]
    public string ShortName { get; set; } = string.Empty;
    [MaxLength(255)]
    public string Address { get; set; } = string.Empty;
    public Guid? AO { get; set; } = null;

    [MaxLength(50)]
    public string Phone { get; set; } = string.Empty;
    [MaxLength(50)]
    public string Fax { get; set; } = string.Empty;
    [MaxLength(13)]
    public string HotLine { get; set; } = string.Empty;
    [MaxLength(60)]
    public string Email { get; set; } = string.Empty;
    [MaxLength(100)]
    public string SiteUrl { get; set; } = string.Empty;

    [MaxLength(50)]
    public string HeadName { get; set; } = string.Empty;
    [MaxLength(50)]
    public string HeadLastName { get; set; } = string.Empty;
    [MaxLength(50)]
    public string HeadMidName { get; set; } = string.Empty;
    public DateTime? Changed { get; set; } = null;
}
