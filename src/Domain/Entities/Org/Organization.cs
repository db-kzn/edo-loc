using EDO_FOMS.Domain.Contracts;
using EDO_FOMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace EDO_FOMS.Domain.Entities.Org
{
    [Index("Inn", IsUnique = false)]
    //[Index("UserId", IsUnique = false)]// Name ="IX_OwnerId")
    public class Organization : AuditableEntity<int>
    {
        //public Organization()
        //{
        //	//Addresses = new List<Address>();
        //	//Cards = new List<Card>();
        //	//Documents = new List<Document>();
        //	//Items = new List<Item>();
        //	//Partners = new List<Partner>();
        //	//Organizations = new List<Partner>();
        //	//Makes = new List<Make>();
        //	//Units = new List<Unit>();
        //	//Sheets = new List<Sheet>();
        //}

        //public virtual ICollection<Address> Addresses { get; set; }
        //public virtual ICollection<Card> Cards { get; set; }
        //public virtual ICollection<Document> Documents { get; set; }
        //public virtual ICollection<Item> Items { get; set; }
        //public virtual ICollection<Partner> Partners { get; set; }
        //public virtual ICollection<Partner> Organizations { get; set; }
        //public virtual ICollection<Make> Makes { get; set; }
        //public virtual ICollection<Unit> Units { get; set; }
        //public virtual ICollection<Sheet> Sheets { get; set; }

        //[Key, Column(Order = 0)]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        [MaxLength(12)]
        public string Inn { get; set; } = string.Empty;
        [MaxLength(13)]
        public string Ogrn { get; set; } = string.Empty;
        [MaxLength(6)]
        public string OmsCode { get; set; } = string.Empty;
        [Required]
        [MaxLength(500)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(32)]
        public string ShortName { get; set; } = string.Empty;

        public string HeadId { get; set; } = string.Empty; // Head - User ID
        public string BuhgId { get; set; } = string.Empty; // Buhg - User ID

        //public string UserId { get; set; } // Owner - User ID
        //public string UserSnils { get; set; } // User SNISL

        public bool IsPublic { get; set; } = true; // CA no public - hide
        public OrgTypes Type { get; set; } = OrgTypes.Undefined; // N/D, FOND, SMO, MO, CA
        public OrgStates State { get; set; } = OrgStates.Undefined; // N/D, OnSubmit, Active, Inactive, Blocked, Closed, Deleted

        [MaxLength(25)]
        public string Phone { get; set; } = string.Empty;
        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;
        //[MaxLength(50)]
        //public string Url { get; set; }

        //[Column(Order = 2)]
        //public int? CardId { get; set; }
        //[ForeignKey("OrganizationId, CardId")]
        //public virtual Card Card { get; set; }

        //[Column(Order = 3)]
        //public int? AddressId { get; set; }
        //[ForeignKey("OrganizationId, AddressId")]
        //public virtual Address Address { get; set; }

        //public int? ParentId { get; set; }
        //[MaxLength(50)]
        //public string WorkArea { get; set; }
        //public int ModeIx { get; set; }

        //[MaxLength(50)]
        //public string LocalName { get; set; }
        //[MaxLength(100)]
        //public string FullName { get; set; }

        //public int AdoptionIx { get; set; }
        //public int CheckLevelIx { get; set; }
        //public bool IsHidden { get; set; }

        //[MaxLength(50)]
        //public string Note { get; set; }
        //public bool IsSeparated { get; set; }
        //public int? DatabaseIx { get; set; }
    }
}
