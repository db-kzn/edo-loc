using EDO_FOMS.Domain.Contracts;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using EDO_FOMS.Application.Interfaces.Chat;
using EDO_FOMS.Application.Models.Chat;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using EDO_FOMS.Domain.Enums;

namespace EDO_FOMS.Infrastructure.Models.Identity
{
    [Index("OrgId", IsUnique = false)]
    public class EdoFomsUser : IdentityUser<string>, IChatUser, IAuditableEntity<string>
    {
        public EdoFomsUser()
        {
            ChatHistoryFromUsers = new HashSet<ChatHistory<EdoFomsUser>>();
            ChatHistoryToUsers = new HashSet<ChatHistory<EdoFomsUser>>();
        }

        public virtual ICollection<ChatHistory<EdoFomsUser>> ChatHistoryFromUsers { get; set; }
        public virtual ICollection<ChatHistory<EdoFomsUser>> ChatHistoryToUsers { get; set; }

        public int OrgId { get; set; }

        [MaxLength(12)]
        public string InnLe { get; set; }
        [MaxLength(11)]
        public string Snils { get; set; }
        [MaxLength(12)]
        public string Inn { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(200)]
        public string Surname { get; set; }
        [MaxLength(200)]
        public string GivenName { get; set; }

        public OrgTypes OrgType { get; set; } = OrgTypes.Undefined;  // N/D, FOND, SMO, MO, CA
        public UserBaseRoles BaseRole { get; set; } = UserBaseRoles.Employee; // N/D, Sys Admin, Chief, Employee
        public bool IsActive { get; set; } = true;

        [Column(TypeName = "text")]
        public string ProfilePictureDataUrl { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}